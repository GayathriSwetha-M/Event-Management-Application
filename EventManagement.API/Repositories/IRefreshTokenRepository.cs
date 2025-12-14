using EventManagement.API.Models;

namespace EventManagement.API.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task DeleteExpiredTokensAsync();
    }
}

