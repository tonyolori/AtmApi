using Application.Common.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validator;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(user => user.Email)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Invalid email format");

        RuleFor(user => user.Password)
          .NotEmpty().WithMessage("Password is required")
          .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
          //.Matches("[a-zA-Z]").WithMessage("Password must contain at least one letter")
          .Matches("[0-9]").WithMessage("Password must contain at least one number")
          .Matches("[!@#$%^&*]").WithMessage("Password must contain at least one special character")
          .NotEqual(user => user.Email).WithMessage("Password cannot be the same as email")
          .Must(BeValidPassword)
          .WithMessage("Invalid Password, Kindly ensure your password is at least 8 characters long, contains 1 digit and 1 special character");

    }

    private static bool BeValidPassword(string password)
    {
        string pattern = @"^(?=.*\d)(?=.*\W)[a-zA-Z\d\W]{8,}$";
        //string pattern = @"^(?=.*\\d)(?=.*[\\W_])(?=.*[a-zA-Z]).{8,}$";

        Regex regex = new Regex(pattern);

        return regex.IsMatch(password);
    }
}


