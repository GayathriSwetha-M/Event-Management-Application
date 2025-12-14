using EventManagement.API.Models;

namespace EventManagement.API.Facades
{
    public interface IAdminFacade
    {
        Task<ApiResponse<DashboardOverviewResponse>> GetDashboardOverviewAsync();
        Task<ApiResponse<EventResponse>> CreateEventAsync(CreateEventRequest request, Guid createdBy);
        Task<ApiResponse<List<EventResponse>>> GetAllEventsAsync();
        Task<ApiResponse<EventResponse>> UpdateEventAsync(Guid id, UpdateEventRequest request);
        Task<ApiResponse<object>> DeleteEventAsync(Guid id);
        Task<ApiResponse<List<object>>> GetAllUsersAsync();
        Task<ApiResponse<List<object>>> GetAllBookingsAsync();
    }
}

