using Application.Users.Commands;
using FluentValidation;

namespace Application.Validator.Commands;
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .Must(x => x == null || x.ToString().Length == 10)
            .WithMessage("Account number must be 10 digits long");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid Email")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x)
            .Custom((command, context) =>
            {
                if (command.AccountNumber == null && string.IsNullOrEmpty(command.Email))
                {
                    context.AddFailure("Either account number or email must be provided");
                }
            });
    }

}
