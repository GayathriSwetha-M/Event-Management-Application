using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventManagement.API.Models;
using EventManagement.API.Facades;
using System.Security.Claims;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventFacade _eventFacade;

        public EventsController(IEventFacade eventFacade)
        {
            _eventFacade = eventFacade;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            var response = await _eventFacade.GetUpcomingEventsAsync();
            
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            var response = await _eventFacade.GetEventByIdAsync(id);
            
            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                    return NotFound(response);
                
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpPost("{id}/book")]
        public async Task<IActionResult> BookEvent(Guid id, [FromBody] BookEventRequest? request = null)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid authentication token"
                });
            }

            // Default to 1 seat if not specified (for backward compatibility)
            var numberOfSeats = request?.NumberOfSeats ?? 1;

            var response = await _eventFacade.BookEventAsync(id, userId, numberOfSeats);
            
            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                    return NotFound(response);
                
                if (response.Message.Contains("already booked") || response.Message.Contains("fully booked") || response.Message.Contains("not enough"))
                    return Conflict(response);
                
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
