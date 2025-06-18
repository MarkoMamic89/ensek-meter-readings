using Ensek.MeterReadings.Api.Data;
using Ensek.MeterReadings.Api.DTOs;
using Ensek.MeterReadings.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ensek.MeterReadings.Api.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly AppDbContext _context;

        public MeterReadingService(AppDbContext context)
        {
            _context = context;
        }

        private record ParsedRow(int LineNumber, MeterReadingCsvRow Row);

        public async Task<UploadSummaryDto> ProcessCsvAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            var parsedRows = new List<ParsedRow>();
            var failureDetails = new List<FailureDetail>();
            int success = 0, failed = 0;
            int lineNumber = 1;

            await reader.ReadLineAsync(); // Skip header

            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                line = line.Trim().TrimEnd(',');
                var parts = line.Split(',');

                if (parts.Length < 3)
                {
                    failed++;
                    failureDetails.Add(new FailureDetail(lineNumber, "Missing data fields"));
                    continue;
                }

                if (!int.TryParse(parts[0], out var accountId))
                {
                    failed++;
                    failureDetails.Add(new FailureDetail(lineNumber, "Invalid AccountId"));
                    continue;
                }

                if (!DateTime.TryParseExact(
                        parts[1],
                        new[] { "dd/MM/yyyy HH:mm", "d/M/yyyy H:mm", "dd/M/yyyy H:mm", "d/MM/yyyy HH:mm" },
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var readingDateTime))
                {
                    failed++;
                    failureDetails.Add(new FailureDetail(lineNumber, $"Invalid date: {parts[1]}"));
                    continue;
                }

                var readingValue = parts[2].Trim();

                parsedRows.Add(new ParsedRow(lineNumber, new MeterReadingCsvRow(accountId, readingDateTime, readingValue)));
            }

            foreach (var entry in parsedRows)
            {
                var rowLine = entry.LineNumber;
                var row = entry.Row;

                if (!_context.Accounts.Any(a => a.Id == row.AccountId))
                {
                    failed++;
                    failureDetails.Add(new FailureDetail(rowLine, "Account not found"));
                    continue;
                }

                if (_context.MeterReadings.Local.Any(m => m.AccountId == row.AccountId && m.ReadingDateTime == row.ReadingDateTime) ||
                    _context.MeterReadings.Any(m => m.AccountId == row.AccountId && m.ReadingDateTime == row.ReadingDateTime))
                {
                    failed++;
                    failureDetails.Add(new FailureDetail(rowLine, "Duplicate meter reading"));
                    continue;
                }

                var existingLatest = _context.MeterReadings
                    .Where(m => m.AccountId == row.AccountId)
                    .OrderByDescending(m => m.ReadingDateTime)
                    .FirstOrDefault();

                if (existingLatest != null && row.ReadingDateTime < existingLatest.ReadingDateTime)
                {
                    failed++;
                    failureDetails.Add(new FailureDetail(rowLine, "Reading is older than existing one"));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row.ReadingValue) || !Regex.IsMatch(row.ReadingValue, @"^\d{5}$"))
                {
                    failed++;
                    failureDetails.Add(new FailureDetail(rowLine, "Reading value not in NNNNN format"));
                    continue;
                }

                _context.MeterReadings.Add(new MeterReading
                {
                    AccountId = row.AccountId,
                    ReadingDateTime = row.ReadingDateTime,
                    ReadingValue = row.ReadingValue
                });

                success++;
            }

            await _context.SaveChangesAsync();
            return new UploadSummaryDto(success, failed, failureDetails);
        }
    }
}
