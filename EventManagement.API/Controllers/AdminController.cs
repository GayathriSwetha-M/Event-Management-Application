using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventManagement.API.Models;
using EventManagement.API.Facades;
using System.Security.Claims;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminFacade _adminFacade;

        public AdminController(IAdminFacade adminFacade)
        {
            _adminFacade = adminFacade;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var response = await _adminFacade.GetDashboardOverviewAsync();
            
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpPost("events")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
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

            var response = await _adminFacade.CreateEventAsync(request, userId);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetAllEvents()
        {
            var response = await _adminFacade.GetAllEventsAsync();
            
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpPut("events/{id}")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
        {
            var response = await _adminFacade.UpdateEventAsync(id, request);
            
            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                    return NotFound(response);
                
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("events/{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var response = await _adminFacade.DeleteEventAsync(id);
            
            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                    return NotFound(response);
                
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _adminFacade.GetAllUsersAsync();
            
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var response = await _adminFacade.GetAllBookingsAsync();
            
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }
    }
}
