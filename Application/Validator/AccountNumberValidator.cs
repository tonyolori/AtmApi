using FluentValidation;

namespace Application.Validator
{
    public class AccountNumberValidator :AbstractValidator<int>
    {
        public AccountNumberValidator()
        {
        }
    }
}
