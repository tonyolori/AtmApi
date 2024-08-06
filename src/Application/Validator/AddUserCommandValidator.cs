using Application.Users.Commands;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validator
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            RuleFor(user => user)
                .NotEmpty()
                .Custom((user, context) =>
                {
                    if (user.User.FirstName.ToLower().Equals(user.User.LastName.ToLower()))
                    {
                        context.AddFailure("First and last name cannot be the same.");
                    }
                });

            RuleFor(user => user.User.FirstName)
                .NotEmpty().WithMessage("FirstName cannot be empty.")
                .Must(BeValidName).WithMessage("Invalid Name");

            RuleFor(user => user.User.LastName)
                .NotEmpty()
                .WithMessage("LastName cannot be empty.")
                .Must(BeValidName).WithMessage("Invalid Name");


            RuleFor(user => user.User.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Invalid {PropertyName} format.");


            RuleFor(user => user.User.Pin)
                .NotEmpty()
                .Must(pin => pin.ToString().Length == 4)
                .WithMessage("Invalid PIN length. PIN must be 4 digits.");

            RuleFor(user => user.User.Password)
                .NotEmpty()
                .Must(BeValidPassword)
                .WithMessage("Invalid Password, Kindly ensure your password is at least 8 characters long, contains 1 digit and 1 special character");
        }
        private bool BeValidName(string? name)
        {
            name = name.Replace(" ", "");
            name = name.Replace("-", "");
            return name.All(char.IsLetter);
        }
        private static bool BeValidPassword(string password)
        {
            string pattern = @"^(?=.*\d)(?=.*\W)[a-zA-Z\d\W]{8,}$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(password);
        }
    }

}



