using Microsoft.EntityFrameworkCore;
using EventManagement.API.Data;

namespace EventManagement.API.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetUpcomingEventsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var now = TimeOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Events
                .Include(e => e.Bookings)
                .Where(e => e.EventDate > today || (e.EventDate == today && e.EventTime >= now))
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.EventTime)
                .ToListAsync();
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Bookings)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Event> CreateAsync(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<Event> UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
                return false;

            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetBookedCountAsync(Guid eventId)
        {
            return await _context.Bookings
                .CountAsync(b => b.EventId == eventId && b.Status == "booked");
        }
    }
}

