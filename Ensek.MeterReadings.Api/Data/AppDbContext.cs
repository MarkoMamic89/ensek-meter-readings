using Ensek.MeterReadings.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensek.MeterReadings.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<MeterReading> MeterReadings => Set<MeterReading>();
    }
}
