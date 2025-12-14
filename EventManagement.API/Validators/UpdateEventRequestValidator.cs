using EventManagement.API.Models;
using FluentValidation;

namespace EventManagement.API.Validators
{
    public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
    {
        public UpdateEventRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(150).WithMessage("Title must not exceed 150 characters");

            RuleFor(x => x.EventDate)
                .NotEmpty().WithMessage("Event date is required");

            RuleFor(x => x.EventTime)
                .NotEmpty().WithMessage("Event time is required");

            RuleFor(x => x.Venue)
                .NotEmpty().WithMessage("Venue is required")
                .MaximumLength(150).WithMessage("Venue must not exceed 150 characters");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0");
        }
    }
}

