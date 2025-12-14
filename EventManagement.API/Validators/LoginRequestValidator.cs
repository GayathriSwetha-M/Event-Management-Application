using EventManagement.API.Models;
using FluentValidation;

namespace EventManagement.API.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.EmailOrPhone)
                .NotEmpty().WithMessage("Email or Phone is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}

