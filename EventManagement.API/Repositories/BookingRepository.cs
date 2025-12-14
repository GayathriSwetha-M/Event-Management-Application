using Microsoft.EntityFrameworkCore;
using EventManagement.API.Data;

namespace EventManagement.API.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking?> GetByUserAndEventAsync(Guid userId, Guid eventId)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.EventId == eventId && b.Status == "booked");
        }

        public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .Include(b => b.Event)
                .Where(b => b.UserId == userId && b.Status == "booked")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Event)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<int> CountByEventIdAsync(Guid eventId)
        {
            return await _context.Bookings
                .CountAsync(b => b.EventId == eventId && b.Status == "booked");
        }

        public async Task<int> SumSeatsByEventIdAsync(Guid eventId)
        {
            return await _context.Bookings
                .Where(b => b.EventId == eventId && b.Status == "booked")
                .SumAsync(b => b.NumberOfSeats);
        }
    }
}

