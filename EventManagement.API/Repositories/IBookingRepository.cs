namespace EventManagement.API.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(Guid id);
        Task<Booking?> GetByUserAndEventAsync(Guid userId, Guid eventId);
        Task<List<Booking>> GetByUserIdAsync(Guid userId);
        Task<List<Booking>> GetAllAsync();
        Task<Booking> CreateAsync(Booking booking);
        Task<int> CountByEventIdAsync(Guid eventId);
        Task<int> SumSeatsByEventIdAsync(Guid eventId);
    }
}

