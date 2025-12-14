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
    public class BookingsController : ControllerBase
    {
        private readonly IBookingFacade _bookingFacade;

        public BookingsController(IBookingFacade bookingFacade)
        {
            _bookingFacade = bookingFacade;
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
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

            var response = await _bookingFacade.GetUserBookingsAsync(userId);
            
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response);
        }
    }
}
