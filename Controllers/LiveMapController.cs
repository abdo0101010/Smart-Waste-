using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.Models;
using SmartWaste.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWaste.Controllers
{
    public class LocationUpdateDto
    {
        public int DriverId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class LiveMapController : ControllerBase
    {
        private readonly ILocationHubService _locationHubService;

        public LiveMapController(ILocationHubService locationHubService)
        {
            _locationHubService = locationHubService;
        }

        [AllowAnonymous]
        [HttpGet("truck-stream/{driverId}")]
        public async Task GetTruckLocationStream(int driverId, CancellationToken cancellationToken)
        {
            Response.ContentType = "text/event-stream";
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            var stream = _locationHubService.GetTruckLocationStream(driverId, cancellationToken);

            await foreach (var data in stream)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }

        [AllowAnonymous]
        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdateDto dto)
        {
            if (dto.Latitude == 0 && dto.Longitude == 0)
            {
                return BadRequest(new { Error = "الداتا وصلت للسيرفر بصيغة null!", DataSent = dto });
            }

            await _locationHubService.UpdateDriverLocation(dto.DriverId, dto.Latitude, dto.Longitude, dto.Status, dto.Capacity);

            return Ok(new
            {
                Message = "تم التحديث بنجاح",
                DataReceived = dto
            });
        }
    }
}