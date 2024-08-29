using Application.Users.Queries;
using FluentValidation;

namespace Application.Validator.Queries;
public class FilterUsersQueryValidator: AbstractValidator<FilterUsersQuery>
{
    public FilterUsersQueryValidator() 
    {
        // null is a valid input for both types so we dont write a validator for that
        RuleFor(x => x.UserRole)
            .InclusiveBetween(0, 2)
            .WithMessage("UserRole must be between 0 and 2");
    }
}

