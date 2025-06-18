using Ensek.MeterReadings.Api.Controllers;
using Ensek.MeterReadings.Api.Data;
using Ensek.MeterReadings.Api.DTOs;
using Ensek.MeterReadings.Api.Models;
using Ensek.MeterReadings.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Xunit;

namespace Ensek.MeterReadings.Tests
{
    public class MeterReadingControllerTests
    {
        private AppDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            context.Accounts.Add(new Account { Id = 1234, FirstName = "Test", LastName = "User" });
            context.SaveChanges();
            return context;
        }

        private IFormFile CreateCsvFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "test.csv");
        }

        [Fact]
        public async Task Upload_ValidCsv_ReturnsCorrectSummary()
        {
            // Arrange
            var context = GetInMemoryContext(); // Already seeds AccountId = 1234
            var service = new MeterReadingService(context);
            var controller = new MeterReadingController(service);

            var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                      "1234,01/05/2024 09:00,12345\n" +
                      "1234,01/05/2024 09:00,12345\n" +  // duplicate
                      "9999,01/05/2024 09:00,12345\n" +  // invalid account
                      "1234,01/05/2024 10:00,ABCDE\n";   // invalid format

            var file = CreateCsvFile(csv);
            var request = new FileUploadRequest { File = file };

            // Act
            var actionResult = await controller.Upload(request);
            var okResult = actionResult.Result as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            var summary = okResult.Value as UploadSummaryDto;
            Assert.NotNull(summary);
            Assert.Equal(1, summary.Successful);
            Assert.Equal(3, summary.Failed);
        }


        [Fact]
        public async Task Upload_NullFile_ReturnsBadRequest()
        {
            // Arrange
            var service = new MeterReadingService(GetInMemoryContext());
            var controller = new MeterReadingController(service);

            // Act
            var actionResult = await controller.Upload(new FileUploadRequest { File = null });
            var result = actionResult.Result as BadRequestObjectResult;
   
            // Assert
            Assert.NotNull(result);
            Assert.Equal("CSV file is required.", result.Value);
        }
    }
}
