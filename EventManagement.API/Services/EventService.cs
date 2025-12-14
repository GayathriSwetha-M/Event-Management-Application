using EventManagement.API.Models;
using EventManagement.API.Repositories;

namespace EventManagement.API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IBookingRepository _bookingRepository;

        public EventService(IEventRepository eventRepository, IBookingRepository bookingRepository)
        {
            _eventRepository = eventRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<List<EventResponse>> GetUpcomingEventsAsync()
        {
            var events = await _eventRepository.GetUpcomingEventsAsync();
            
            return events.Select(e => 
            {
                var bookedSeats = e.Bookings.Where(b => b.Status == "booked").Sum(b => b.NumberOfSeats);
                return new EventResponse
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    EventTime = e.EventTime,
                    Venue = e.Venue,
                    Capacity = e.Capacity,
                    BookedCount = bookedSeats,
                    AvailableSlots = e.Capacity - bookedSeats
                };
            }).ToList();
        }

        public async Task<EventResponse> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            var bookedSeats = eventEntity.Bookings.Where(b => b.Status == "booked").Sum(b => b.NumberOfSeats);

            return new EventResponse
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                EventDate = eventEntity.EventDate,
                EventTime = eventEntity.EventTime,
                Venue = eventEntity.Venue,
                Capacity = eventEntity.Capacity,
                BookedCount = bookedSeats,
                AvailableSlots = eventEntity.Capacity - bookedSeats
            };
        }

        public async Task<EventResponse> CreateEventAsync(CreateEventRequest request, Guid createdBy)
        {
            var eventEntity = new Event
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                EventDate = request.EventDate,
                EventTime = request.EventTime,
                Venue = request.Venue,
                Capacity = request.Capacity,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdEvent = await _eventRepository.CreateAsync(eventEntity);

            return new EventResponse
            {
                Id = createdEvent.Id,
                Title = createdEvent.Title,
                Description = createdEvent.Description,
                EventDate = createdEvent.EventDate,
                EventTime = createdEvent.EventTime,
                Venue = createdEvent.Venue,
                Capacity = createdEvent.Capacity,
                BookedCount = 0,
                AvailableSlots = createdEvent.Capacity
            };
        }

        public async Task<EventResponse> UpdateEventAsync(Guid id, UpdateEventRequest request)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            // Check capacity - sum of all booked seats
            var totalBookedSeats = await _bookingRepository.SumSeatsByEventIdAsync(id);
            if (request.Capacity < totalBookedSeats)
            {
                throw new ArgumentException($"Cannot set capacity below current booked seats ({totalBookedSeats})");
            }

            eventEntity.Title = request.Title;
            eventEntity.Description = request.Description ?? string.Empty;
            eventEntity.EventDate = request.EventDate;
            eventEntity.EventTime = request.EventTime;
            eventEntity.Venue = request.Venue;
            eventEntity.Capacity = request.Capacity;

            var updatedEvent = await _eventRepository.UpdateAsync(eventEntity);

            return new EventResponse
            {
                Id = updatedEvent.Id,
                Title = updatedEvent.Title,
                Description = updatedEvent.Description,
                EventDate = updatedEvent.EventDate,
                EventTime = updatedEvent.EventTime,
                Venue = updatedEvent.Venue,
                Capacity = updatedEvent.Capacity,
                BookedCount = totalBookedSeats,
                AvailableSlots = updatedEvent.Capacity - totalBookedSeats
            };
        }

        public async Task DeleteEventAsync(Guid id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            var totalBookedSeats = await _bookingRepository.SumSeatsByEventIdAsync(id);
            if (totalBookedSeats > 0)
            {
                throw new InvalidOperationException($"Cannot delete event with {totalBookedSeats} booked seat(s)");
            }

            await _eventRepository.DeleteAsync(id);
        }

        public async Task<List<EventResponse>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            
            return events.Select(e => 
            {
                var bookedSeats = e.Bookings.Where(b => b.Status == "booked").Sum(b => b.NumberOfSeats);
                return new EventResponse
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    EventTime = e.EventTime,
                    Venue = e.Venue,
                    Capacity = e.Capacity,
                    BookedCount = bookedSeats,
                    AvailableSlots = e.Capacity - bookedSeats
                };
            }).ToList();
        }
    }
}

