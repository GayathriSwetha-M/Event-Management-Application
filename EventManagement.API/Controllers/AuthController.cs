using Microsoft.AspNetCore.Mvc;
using EventManagement.API.Models;
using EventManagement.API.Facades;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthFacade _authFacade;

        public AuthController(IAuthFacade authFacade)
        {
            _authFacade = authFacade;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            var response = await _authFacade.SignupAsync(request);
            
            if (!response.Success)
            {
                if (response.Message.Contains("already exists"))
                    return Conflict(response);
                
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authFacade.LoginAsync(request);
            
            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _authFacade.RefreshTokenAsync(request);
            
            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}
