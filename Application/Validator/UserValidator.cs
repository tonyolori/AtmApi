using Domain.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validator
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
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
                .NotEmpty().WithMessage("FirstName cannot be empty.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("LastName cannot be empty.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Invalid {PropertyName} format.");


            RuleFor(user => user.Pin)
                .NotEmpty()
                .Must(pin => pin.ToString().Length == 4)
                .WithMessage("Invalid PIN length. PIN must be 4 digits.");

            RuleFor(user => user.Password)
                .NotEmpty()
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

}



