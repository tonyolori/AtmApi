using Application.UserTransactions.Commands;
using FluentValidation;

namespace Application.Validator.Commands;
public class ChangePinCommandValidator : AbstractValidator<ChangePinCommand>
{
    public ChangePinCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .Must(x => x.ToString().Length == 10)
            .WithMessage("Account number must be 10 digits long");

        RuleFor(x => x.NewPin)
            .Must(x => x.ToString().Length == 4)
            .WithMessage("New pin must be 4 digits long");
    }
}
