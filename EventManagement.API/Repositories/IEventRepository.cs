namespace EventManagement.API.Repositories
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id);
        Task<List<Event>> GetUpcomingEventsAsync();
        Task<List<Event>> GetAllAsync();
        Task<Event> CreateAsync(Event eventEntity);
        Task<Event> UpdateAsync(Event eventEntity);
        Task<bool> DeleteAsync(Guid id);
        Task<int> GetBookedCountAsync(Guid eventId);
    }
}

