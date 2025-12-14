using EventManagement.API.Models;
using EventManagement.API.Repositories;

namespace EventManagement.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventRepository _eventRepository;

        public BookingService(IBookingRepository bookingRepository, IEventRepository eventRepository)
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
        }

        public async Task<object> BookEventAsync(Guid eventId, Guid userId, int numberOfSeats = 1)
        {
            // Validate number of seats
            if (numberOfSeats < 1)
            {
                throw new ArgumentException("Number of seats must be at least 1");
            }

            // Check if event exists
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            // Check if event is in the past
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var now = TimeOnly.FromDateTime(DateTime.UtcNow);
            if (eventEntity.EventDate < today || (eventEntity.EventDate == today && eventEntity.EventTime < now))
            {
                throw new InvalidOperationException("Cannot book past events");
            }

            // Check if user already booked this event
            var existingBooking = await _bookingRepository.GetByUserAndEventAsync(userId, eventId);
            if (existingBooking != null)
            {
                throw new InvalidOperationException("You have already booked this event. You can update your booking instead.");
            }

            // Check capacity - sum of all booked seats
            var totalBookedSeats = await _bookingRepository.SumSeatsByEventIdAsync(eventId);
            if (totalBookedSeats + numberOfSeats > eventEntity.Capacity)
            {
                var availableSeats = eventEntity.Capacity - totalBookedSeats;
                throw new InvalidOperationException($"Not enough seats available. Only {availableSeats} seat(s) remaining.");
            }

            // Create booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EventId = eventId,
                Status = "booked",
                NumberOfSeats = numberOfSeats,
                CreatedAt = DateTime.UtcNow
            };

            var createdBooking = await _bookingRepository.CreateAsync(booking);

            return new
            {
                Id = createdBooking.Id,
                BookingId = createdBooking.Id,
                EventId = eventEntity.Id,
                EventTitle = eventEntity.Title,
                EventDate = eventEntity.EventDate,
                EventTime = eventEntity.EventTime,
                Venue = eventEntity.Venue,
                NumberOfSeats = numberOfSeats
            };
        }

        public async Task<List<BookingResponse>> GetUserBookingsAsync(Guid userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var now = TimeOnly.FromDateTime(DateTime.UtcNow);

            return bookings.Select(b => new BookingResponse
            {
                Id = b.Id,
                EventId = b.Event.Id,
                EventTitle = b.Event.Title,
                EventDate = b.Event.EventDate,
                EventTime = b.Event.EventTime,
                Venue = b.Event.Venue,
                Status = (b.Event.EventDate < today || (b.Event.EventDate == today && b.Event.EventTime < now))
                    ? "completed"
                    : "upcoming",
                NumberOfSeats = b.NumberOfSeats,
                CreatedAt = b.CreatedAt
            }).ToList();
        }
    }
}

