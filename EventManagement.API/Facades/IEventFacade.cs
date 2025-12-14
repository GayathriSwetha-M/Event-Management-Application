using EventManagement.API.Models;

namespace EventManagement.API.Facades
{
    public interface IEventFacade
    {
        Task<ApiResponse<List<EventResponse>>> GetUpcomingEventsAsync();
        Task<ApiResponse<EventResponse>> GetEventByIdAsync(Guid id);
        Task<ApiResponse<object>> BookEventAsync(Guid eventId, Guid userId, int numberOfSeats = 1);
    }
}

