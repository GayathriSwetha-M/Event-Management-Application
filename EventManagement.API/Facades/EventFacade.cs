using EventManagement.API.Models;
using EventManagement.API.Services;

namespace EventManagement.API.Facades
{
    public class EventFacade : IEventFacade
    {
        private readonly IEventService _eventService;
        private readonly IBookingService _bookingService;

        public EventFacade(IEventService eventService, IBookingService bookingService)
        {
            _eventService = eventService;
            _bookingService = bookingService;
        }

        public async Task<ApiResponse<List<EventResponse>>> GetUpcomingEventsAsync()
        {
            try
            {
                var events = await _eventService.GetUpcomingEventsAsync();
                return new ApiResponse<List<EventResponse>>
                {
                    Success = true,
                    Message = "Events retrieved successfully",
                    Data = events
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<EventResponse>>
                {
                    Success = false,
                    Message = "An error occurred while fetching events",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<EventResponse>> GetEventByIdAsync(Guid id)
        {
            try
            {
                var eventResponse = await _eventService.GetEventByIdAsync(id);
                return new ApiResponse<EventResponse>
                {
                    Success = true,
                    Message = "Event retrieved successfully",
                    Data = eventResponse
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new ApiResponse<EventResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EventResponse>
                {
                    Success = false,
                    Message = "An error occurred while fetching event",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<object>> BookEventAsync(Guid eventId, Guid userId, int numberOfSeats = 1)
        {
            try
            {
                var result = await _bookingService.BookEventAsync(eventId, userId, numberOfSeats);
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Booking successful",
                    Data = result
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (InvalidOperationException ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while booking event",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}

