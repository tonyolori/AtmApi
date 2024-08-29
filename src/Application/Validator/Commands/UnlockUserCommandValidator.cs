using Application.Users.Commands;
using FluentValidation;

namespace Application.Validator.Commands;
public class UnlockUserCommandValidator : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
