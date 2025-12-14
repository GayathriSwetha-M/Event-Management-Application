using EventManagement.API.Models;
using EventManagement.API.Services;

namespace EventManagement.API.Facades
{
    public class BookingFacade : IBookingFacade
    {
        private readonly IBookingService _bookingService;

        public BookingFacade(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<ApiResponse<List<BookingResponse>>> GetUserBookingsAsync(Guid userId)
        {
            try
            {
                var bookings = await _bookingService.GetUserBookingsAsync(userId);
                return new ApiResponse<List<BookingResponse>>
                {
                    Success = true,
                    Message = "Bookings retrieved successfully",
                    Data = bookings
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<BookingResponse>>
                {
                    Success = false,
                    Message = "An error occurred while fetching bookings",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}

