using EventManagement.API.Models;

namespace EventManagement.API.Services
{
    public interface IAdminService
    {
        Task<DashboardOverviewResponse> GetDashboardOverviewAsync();
        Task<List<object>> GetAllUsersAsync();
        Task<List<object>> GetAllBookingsAsync();
    }
}

