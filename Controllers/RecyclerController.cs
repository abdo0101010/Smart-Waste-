using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.TicketSDTOS;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace SmartWaste.Controllers
{ 
    [ApiController]
        [Route("api/[controller]")]
    public class RecyclerController:ControllerBase
    {
        ISupportTicketsServices _supportTicketsServices;
        public RecyclerController(ISupportTicketsServices supportTicketsService)
        {
             _supportTicketsServices = supportTicketsService;
        }
        [HttpGet("tickets/{recyclerId}")]
        [SwaggerOperation(
            Summary = "Get support tickets for a recycler",
            Description = "Retrieves a list of support tickets assigned to a specific recycler, with an optional status filter (e.g., Open, Closed, ALL).",
            OperationId = "GetRecyclerSupportTickets",
            Tags = new[] { "Recycler" }
        )]
        [SwaggerResponse(200, "Returns the list of tickets for the specified recycler", typeof(List<TicketDTO>))]
        [SwaggerResponse(400, "Bad Request or invalid status")]
        [SwaggerResponse(404, "No tickets found for this recycler")]
        public IActionResult GetRecyclerTickets(int recyclerId, [FromQuery] string status = "ALL")
        {
            // بننادي على السيرفيس تجيب التذاكر
            var tickets = _supportTicketsServices.GetRecyclerSupportTickets(recyclerId, status);

            if (tickets == null)
            {
                return NotFound(new { message = "No tickets found for this recycler." });
            }

            return Ok(tickets);
        }

    }
}
