using EventManagement.API.Models;
using EventManagement.API.Services;
using EventManagement.API.Validators;

namespace EventManagement.API.Facades
{
    public class AdminFacade : IAdminFacade
    {
        private readonly IAdminService _adminService;
        private readonly IEventService _eventService;
        private readonly CreateEventRequestValidator _createEventValidator;
        private readonly UpdateEventRequestValidator _updateEventValidator;

        public AdminFacade(
            IAdminService adminService,
            IEventService eventService,
            CreateEventRequestValidator createEventValidator,
            UpdateEventRequestValidator updateEventValidator)
        {
            _adminService = adminService;
            _eventService = eventService;
            _createEventValidator = createEventValidator;
            _updateEventValidator = updateEventValidator;
        }

        public async Task<ApiResponse<DashboardOverviewResponse>> GetDashboardOverviewAsync()
        {
            try
            {
                var overview = await _adminService.GetDashboardOverviewAsync();
                return new ApiResponse<DashboardOverviewResponse>
                {
                    Success = true,
                    Message = "Dashboard overview retrieved successfully",
                    Data = overview
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<DashboardOverviewResponse>
                {
                    Success = false,
                    Message = "An error occurred while fetching overview",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<EventResponse>> CreateEventAsync(CreateEventRequest request, Guid createdBy)
        {
            // Validate request
            var validationResult = await _createEventValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new ApiResponse<EventResponse>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }

            try
            {
                var result = await _eventService.CreateEventAsync(request, createdBy);
                return new ApiResponse<EventResponse>
                {
                    Success = true,
                    Message = "Event created successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EventResponse>
                {
                    Success = false,
                    Message = "An error occurred while creating event",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<EventResponse>>> GetAllEventsAsync()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
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

        public async Task<ApiResponse<EventResponse>> UpdateEventAsync(Guid id, UpdateEventRequest request)
        {
            // Validate request
            var validationResult = await _updateEventValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new ApiResponse<EventResponse>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }

            try
            {
                var result = await _eventService.UpdateEventAsync(id, request);
                return new ApiResponse<EventResponse>
                {
                    Success = true,
                    Message = "Event updated successfully",
                    Data = result
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
            catch (ArgumentException ex)
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
                    Message = "An error occurred while updating event",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteEventAsync(Guid id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Event deleted successfully"
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
                    Message = "An error occurred while deleting event",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<object>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return new ApiResponse<List<object>>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = users
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<object>>
                {
                    Success = false,
                    Message = "An error occurred while fetching users",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<List<object>>> GetAllBookingsAsync()
        {
            try
            {
                var bookings = await _adminService.GetAllBookingsAsync();
                return new ApiResponse<List<object>>
                {
                    Success = true,
                    Message = "Bookings retrieved successfully",
                    Data = bookings
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<object>>
                {
                    Success = false,
                    Message = "An error occurred while fetching bookings",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}

