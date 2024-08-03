using Domain.Entities;
using FluentValidation;

namespace Application.Validator
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MinimumLength(2).WithMessage("First name must be at least 2 characters long");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters long");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[!@#$%^&*]").WithMessage("Password must contain at least one special character");

            RuleFor(user => user.Pin.ToString())
                .NotEmpty().WithMessage("PIN is required")
                .MinimumLength(4).WithMessage("PIN must be at least 4 digits long")
                .MaximumLength(4).WithMessage("PIN cannot be longer than 4 digits");
        }
    }
}
