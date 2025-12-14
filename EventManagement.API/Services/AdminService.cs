using EventManagement.API.Models;
using EventManagement.API.Repositories;

namespace EventManagement.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;

        public AdminService(
            IEventRepository eventRepository,
            IUserRepository userRepository,
            IBookingRepository bookingRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<DashboardOverviewResponse> GetDashboardOverviewAsync()
        {
            var totalEvents = await _eventRepository.GetAllAsync();
            var totalUsers = await _userRepository.GetAllAsync();
            var totalBookings = await _bookingRepository.GetAllAsync();

            return new DashboardOverviewResponse
            {
                TotalEvents = totalEvents.Count,
                TotalUsers = totalUsers.Count(u => u.Role == "user"),
                TotalBookings = totalBookings.Count(b => b.Status == "booked")
            };
        }

        public async Task<List<object>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var bookings = await _bookingRepository.GetAllAsync();

            return users.Where(x => x.Role == "user").Select(u => new
            {
                Id = u.Id,
                Name = u.Name,
                EmailOrPhone = u.EmailOrPhone,
                Role = u.Role,
                Status = "active",
                CreatedAt = u.CreatedAt,
                TotalBookings = bookings.Count(b => b.UserId == u.Id && b.Status == "booked")
            }).Cast<object>().ToList();
        }

        public async Task<List<object>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();

            return bookings.Select(b => new
            {
                Id = b.Id,
                UserId = b.UserId,
                UserName = b.User.Name,
                UserEmail = b.User.EmailOrPhone,
                EventId = b.EventId,
                EventTitle = b.Event.Title,
                EventDate = b.Event.EventDate,
                EventTime = b.Event.EventTime,
                Venue = b.Event.Venue,
                Status = b.Status,
                NumberOfSeats = b.NumberOfSeats,
                CreatedAt = b.CreatedAt
            }).Cast<object>().ToList();
        }
    }
}

