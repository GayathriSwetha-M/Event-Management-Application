using EventManagement.API.Models;

namespace EventManagement.API.Services
{
    public interface IBookingService
    {
        Task<object> BookEventAsync(Guid eventId, Guid userId, int numberOfSeats = 1);
        Task<List<BookingResponse>> GetUserBookingsAsync(Guid userId);
    }
}

