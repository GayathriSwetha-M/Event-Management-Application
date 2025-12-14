using EventManagement.API.Models;

namespace EventManagement.API.Facades
{
    public interface IBookingFacade
    {
        Task<ApiResponse<List<BookingResponse>>> GetUserBookingsAsync(Guid userId);
    }
}

