using EventManagement.API.Models;
using FluentValidation;

namespace EventManagement.API.Validators
{
    public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
    {
        public CreateEventRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(150).WithMessage("Title must not exceed 150 characters");

            RuleFor(x => x.EventDate)
                .NotEmpty().WithMessage("Event date is required")
                .Must(BeFutureDate).WithMessage("Event date must be in the future");

            RuleFor(x => x.EventTime)
                .NotEmpty().WithMessage("Event time is required");

            RuleFor(x => x.Venue)
                .NotEmpty().WithMessage("Venue is required")
                .MaximumLength(150).WithMessage("Venue must not exceed 150 characters");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0");
        }

        private bool BeFutureDate(DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return date >= today;
        }
    }
}

