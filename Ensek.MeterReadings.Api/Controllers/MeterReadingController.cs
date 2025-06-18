using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ensek.MeterReadings.Api.Services;
using Ensek.MeterReadings.Api.DTOs;

namespace Ensek.MeterReadings.Api.Controllers
{
    [ApiController]
    [Route("meter-reading-uploads")]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _service;

        public MeterReadingController(IMeterReadingService service)
        {
            _service = service;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UploadSummaryDto>> Upload([FromForm] FileUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("CSV file is required.");

            var result = await _service.ProcessCsvAsync(request.File);
            return Ok(result);
        }

    }
}
