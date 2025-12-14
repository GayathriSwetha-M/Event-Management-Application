using EventManagement.API.Models;

namespace EventManagement.API.Services
{
    using EventManagement.API.Models;

    public interface IAuthService
    {
        Task<LoginResponse> SignupAsync(SignupRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken);
    }
}

