using BCrypt.Net;
using EventManagement.API.Models;
using EventManagement.API.Repositories;
using EventManagement.API.JWT;

namespace EventManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository, 
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> SignupAsync(SignupRequest request)
        {
            var existingUser = await _userRepository.GetByEmailOrPhoneAsync(request.EmailOrPhone);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email/phone already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                EmailOrPhone = request.EmailOrPhone,
                PasswordHash = passwordHash,
                Role = "user",
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);

            var accessToken = _jwtService.GenerateAccessToken(createdUser.Id, createdUser.EmailOrPhone, createdUser.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = createdUser.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = createdUser.Id,
                Name = createdUser.Name,
                EmailOrPhone = createdUser.EmailOrPhone,
                Role = createdUser.Role
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Find user
            var user = await _userRepository.GetByEmailOrPhoneAsync(request.EmailOrPhone);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email/phone or password");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email/phone or password");
            }

            // Revoke all existing refresh tokens for this user (token rotation)
            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id);

            // Generate new tokens
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.EmailOrPhone, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token to database (expires in 7 days)
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = user.Id,
                Name = user.Name,
                EmailOrPhone = user.EmailOrPhone,
                Role = user.Role
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            // Get principal from expired access token
            var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                throw new UnauthorizedAccessException("Invalid access token");
            }

            // Get user ID from claims
            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid token claims");
            }

            // Verify refresh token exists and is valid
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.UserId != userId || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            // Get user to get role and email
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            // Revoke old refresh token (token rotation for security)
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user.Id, user.EmailOrPhone, user.Role);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Save new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(newRefreshTokenEntity);

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}

