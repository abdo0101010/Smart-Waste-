using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.RecuclerDTOS;
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
        private readonly ISupportTicketsServices _supportTicketsServices;
        private readonly IRecyclerService _recyclerService;
        public RecyclerController(ISupportTicketsServices supportTicketsService, IRecyclerService recyclerService)
        {
             _supportTicketsServices = supportTicketsService;
              _recyclerService = recyclerService;
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

        [HttpPut("update/{id:int}")]
        [SwaggerOperation(
            Summary = "Updates recycler profile information",
            Description = "Allows recyclers to update their profile name, phone, and vehicle information.",
            OperationId = "UpdateRecyclerProfile",
            Tags = new[] { "Recycler" }
        )]
        [SwaggerResponse(200, "Profile updated successfully")]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(404, "Recycler not found")]
        public async Task<IActionResult> UpdateRecyclerAsync([FromRoute] int id, [FromBody] RecyclerUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _recyclerService.UpdateRecyclerAsync(id, dto);

            if (!result)
            {
                return NotFound(new { message = $"Recycler with ID {id} not found." });
            }

            return Ok(new { message = "Profile updated successfully." });
        }
        [HttpPut("/api/recycler/update-profile-picture/{id:int}")]
        [SwaggerOperation(
         Summary = "Updates the profile picture for a recycler",
         Description = "Uploads a new picture, deletes the old one from server, and updates database path",
         Tags = new[] { "Recyclers" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Profile picture updated successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Recycler not found")]
        public async Task<IActionResult> UpdateRecyclerProfilePicture([FromRoute] int id, [FromForm] UploadProfilePictureDTO dto)
        {
            if (dto == null || dto.File == null || dto.File.Length == 0)
            {
                return BadRequest(new { message = "Please provide a valid image file." });
            }

            try
            {
                string newImagePath = await _recyclerService.UpdateRecyclerProfilePictureAsync(id, dto.File);

                return Ok(new
                {
                    message = "Profile picture updated successfully! ",
                    profilePictureUrl = newImagePath
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while uploading the profile picture.", error = ex.Message });
            }      
        }
    }
}
