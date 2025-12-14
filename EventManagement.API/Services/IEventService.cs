using EventManagement.API.Models;

namespace EventManagement.API.Services
{
    public interface IEventService
    {
        Task<List<EventResponse>> GetUpcomingEventsAsync();
        Task<EventResponse> GetEventByIdAsync(Guid id);
        Task<EventResponse> CreateEventAsync(CreateEventRequest request, Guid createdBy);
        Task<EventResponse> UpdateEventAsync(Guid id, UpdateEventRequest request);
        Task DeleteEventAsync(Guid id);
        Task<List<EventResponse>> GetAllEventsAsync();
    }
}

