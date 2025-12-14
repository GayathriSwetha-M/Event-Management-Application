using System.Security.Claims;

namespace EventManagement.API.JWT
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, string emailOrPhone, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
