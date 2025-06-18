using Ensek.MeterReadings.Api.Data;
using Ensek.MeterReadings.Api.Models;

namespace Ensek.MeterReadings.Api.Services
{
    public class AccountSeeder : IAccountSeeder
    {
        private readonly AppDbContext _context;

        public AccountSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.Accounts.Any())
                return;

            if (!File.Exists("Test_Accounts.csv"))
                throw new FileNotFoundException("Test_Accounts.csv file not found in root folder.");

            var lines = await File.ReadAllLinesAsync("Test_Accounts.csv");

            foreach (var line in lines.Skip(1)) // skip header
            {
                var parts = line.Split(',');

                // Validate first 3 columns
                if (parts.Length < 3) continue;

                if (!int.TryParse(parts[0], out var accountId)) continue;

                _context.Accounts.Add(new Account
                {
                    Id = accountId,
                    FirstName = parts[1],
                    LastName = parts[2]
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
