using Ensek.MeterReadings.Api.Data;
using Ensek.MeterReadings.Api.DTOs;
using Ensek.MeterReadings.Api.Models;
using Ensek.MeterReadings.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Xunit;

namespace Ensek.MeterReadings.Tests
{
    public class MeterReadingServiceTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new AppDbContext(options);
            context.Accounts.Add(new Account { Id = 1234, FirstName = "Test", LastName = "User" });
            context.MeterReadings.Add(new MeterReading
            {
                AccountId = 1234,
                ReadingDateTime = new DateTime(2024, 5, 1, 9, 0, 0),
                ReadingValue = "12345"
            });
            context.SaveChanges();
            return context;
        }

        private IFormFile CreateCsvFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "test.csv");
        }

        [Fact]
        public async Task ProcessCsvAsync_ValidAndInvalidRows_ReturnsCorrectSummary()
        {
            var context = GetInMemoryContext();
            var service = new MeterReadingService(context);

            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                      "1234,01/05/2024 10:00,12345\n" +  // valid
                      "1234,01/05/2024 09:00,12345\n" +  // duplicate
                      "9999,01/05/2024 10:00,12345\n" +  // invalid account
                      "1234,01/05/2024 08:00,12345\n" +  // older than existing
                      "1234,01/05/2024 11:00,ABCDE";     // invalid format

            var file = CreateCsvFile(csv.Replace("\n", "\r\n"));

            var result = await service.ProcessCsvAsync(file);

            Assert.Equal(1, result.Successful);
            Assert.Equal(4, result.Failed);
            Assert.Equal(4, result.Failures.Count);
        }

        [Fact]
        public async Task ProcessCsvAsync_EmptyFile_ReturnsZero()
        {
            var context = GetInMemoryContext();
            var service = new MeterReadingService(context);

            var csv = "AccountId,MeterReadingDateTime,MeterReadValue";
            var file = CreateCsvFile(csv);

            var result = await service.ProcessCsvAsync(file);

            Assert.Equal(0, result.Successful);
            Assert.Equal(0, result.Failed);
        }

        [Fact]
        public async Task ProcessCsvAsync_InvalidDateFormat_Fails()
        {
            var context = GetInMemoryContext();
            var service = new MeterReadingService(context);

            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                      "1234,2024-05-01 09:00,12345";
            var file = CreateCsvFile(csv.Replace("\n", "\r\n"));

            var result = await service.ProcessCsvAsync(file);

            Assert.Equal(0, result.Successful);
            Assert.Equal(1, result.Failed);
            Assert.Contains(result.Failures, f => f.Reason.Contains("Invalid date"));
        }
    }
}