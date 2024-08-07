using Application.Users.Commands;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validator;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(login => login.Email)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Invalid email format");

        RuleFor(login => login.Password)
          .NotEmpty().WithMessage("Password is required")
          .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
          //.Matches("[a-zA-Z]").WithMessage("Password must contain at least one letter")
          .Matches("[0-9]").WithMessage("Password must contain at least one number")
          .Matches("[!@#$%^&*]").WithMessage("Password must contain at least one special character")
          .NotEqual(user => user.Email).WithMessage("Password cannot be the same as email");
          
    }
}


