using EventManagement.API.Models;
using FluentValidation;

namespace EventManagement.API.Validators
{
    public class SignupRequestValidator : AbstractValidator<SignupRequest>
    {
        public SignupRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.EmailOrPhone)
                .NotEmpty().WithMessage("Email or Phone is required")
                .MaximumLength(100).WithMessage("Email or Phone must not exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}

