using EventManagement.API.Models;

namespace EventManagement.API.Facades
{
    using EventManagement.API.Models;

    public interface IAuthFacade
    {
        Task<ApiResponse<LoginResponse>> SignupAsync(SignupRequest request);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    }
}

