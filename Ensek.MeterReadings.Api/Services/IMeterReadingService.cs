using Microsoft.AspNetCore.Http;
using Ensek.MeterReadings.Api.DTOs;

namespace Ensek.MeterReadings.Api.Services
{
    public interface IMeterReadingService
    {
        Task<UploadSummaryDto> ProcessCsvAsync(IFormFile file);
    }
}
