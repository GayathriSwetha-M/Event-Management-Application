using EventManagement.API.Models;
using EventManagement.API.Services;
using EventManagement.API.Validators;

namespace EventManagement.API.Facades
{
    public class AuthFacade : IAuthFacade
    {
        private readonly IAuthService _authService;
        private readonly SignupRequestValidator _signupValidator;
        private readonly LoginRequestValidator _loginValidator;

        public AuthFacade(
            IAuthService authService,
            SignupRequestValidator signupValidator,
            LoginRequestValidator loginValidator)
        {
            _authService = authService;
            _signupValidator = signupValidator;
            _loginValidator = loginValidator;
        }

        public async Task<ApiResponse<LoginResponse>> SignupAsync(SignupRequest request)
        {
            var validationResult = await _signupValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }

            try
            {
                var result = await _authService.SignupAsync(request);
                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = result
                };
            }
            catch (InvalidOperationException ex)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            // Validate request
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }

            try
            {
                var result = await _authService.LoginAsync(request);
                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = result
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
                return new ApiResponse<RefreshTokenResponse>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = result
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ApiResponse<RefreshTokenResponse>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RefreshTokenResponse>
                {
                    Success = false,
                    Message = "An error occurred while refreshing token",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}

