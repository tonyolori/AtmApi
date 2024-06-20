using Domain.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validator
{
    public class UserValidator :AbstractValidator<User>
    {
        public UserValidator()
        {
            // Combine NotEmpty and custom logic for first and last name validation
            RuleFor(user => user)
                .NotEmpty()
                .Custom((user, context) => 
                {
                    if (user.FirstName.ToLower().Equals(user.LastName.ToLower()))
                    {
                        context.AddFailure("First and last name cannot be the same.");
                    }
                });

            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("FirstName cannot be empty");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("LastName cannot be empty");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .Custom((email, context) =>
                {
                    if (!IsValidEmail(email))
                    {
                        context.AddFailure("Invalid Email format");
                    }
                }
                );
            

            RuleFor(user => user.Pin)
                .NotEmpty()
                .Must(pin => pin.ToString().Length == 4)
                .WithMessage("Invalid PIN length. PIN must be 4 digits.");

            RuleFor(user => user.Password)
                .NotEmpty()
                .Must(Password => Password.Length >= 8)
                .WithMessage("Password must be at least 8 characters long.")
                .Must(password => !password.Any(char.IsDigit))
                .WithMessage("Password must contain at least one digit.")
                .Must(password => !password.Any(IsSpecialCharacter))
                .WithMessage("Password must contain at least one special character.");


            RuleFor(user => user.AccountNumber)
                .NotEmpty()
                .Custom((accountNumber,context) =>
                {
                    if (accountNumber.ToString().Length != 10)
                    {
                        context.AddFailure("Account number must be 10 characters long.");
                    }
                });

        }
        private static bool IsValidEmail(string email)
        {
            // Regular expression pattern for "_@." format
            string pattern = @"^[\w-]+@[\w-]+\.[\w.]+$";

            //string pattern = @"^\w + ([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            // Check if the email matches the pattern
            return Regex.IsMatch(email, pattern);
        }
        private bool IsSpecialCharacter(char c)
        {
            return !char.IsLetterOrDigit(c);
        }
    }

}



