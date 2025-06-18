using Microsoft.AspNetCore.Http;

namespace Ensek.MeterReadings.Api.DTOs
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
