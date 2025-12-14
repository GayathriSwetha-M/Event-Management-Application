using Microsoft.AspNetCore.Mvc;
using EventManagement.API.Models;
using EventManagement.API.Repositories;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminSetupController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AdminSetupController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailOrPhoneAsync(request.EmailOrPhone);
            if (existingUser != null)
            {
                // If user exists, update role to admin
                existingUser.Role = "admin";
                await _userRepository.UpdateAsync(existingUser);
                
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User role updated to admin successfully"
                });
            }

            // Create new admin user
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                EmailOrPhone = request.EmailOrPhone,
                PasswordHash = passwordHash,
                Role = "admin",
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(adminUser);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Admin user created successfully"
            });
        }
    }

    public class CreateAdminRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string EmailOrPhone { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}

