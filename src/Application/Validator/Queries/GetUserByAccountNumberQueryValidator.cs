using Application.Users.Commands;
using Application.Users.Queries;
using FluentValidation;

namespace Application.Validator.Queries;
public class GetUserByAccountNumberQueryValidator : AbstractValidator<GetUserByAccountNumberQuery>
{
    public GetUserByAccountNumberQueryValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotNull().WithMessage("Account number cannot be empty")
            .Must(x => x.ToString().Length == 10)
            .WithMessage("Account number must be 10 digits long");
    }
}
