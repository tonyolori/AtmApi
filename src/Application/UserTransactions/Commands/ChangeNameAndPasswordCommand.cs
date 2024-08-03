using Application.Common;
using Application.Common.Models;
using Application.Interfaces;
using Application.Users.Commands;
using Application.Validator;
using Domain.Entities;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserTransactions.Commands
{
    public class ChangeNameAndPasswordCommand : IRequest<Result>
    {
        public UserDto User { get; set; }
    }

    public class ChangeNameAndPasswordCommandHandler(UserManager<User> userManager, ISecretHasherService secretHasher) : IRequestHandler<ChangeNameAndPasswordCommand, Result>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ISecretHasherService _secretHasher = secretHasher;
        public async Task<Result> Handle(ChangeNameAndPasswordCommand request, CancellationToken cancellationToken)
        {
            UserDtoValidator validator = new();

            ValidationResult result = validator.Validate(request.User);

            if (!result.IsValid)
            {
                return Result.Failure(request, Utils.GetPrintableErrorString(result.Errors));
            }

            User? dbUser = await _userManager.FindByEmailAsync(request.User.Email);

            if (dbUser == null)
            {
                return Result.Failure(request, "user not found");
            }

            string newPassword = _secretHasher.Hash(request.User.Password);

            if (newPassword == dbUser.PasswordHash)
            {
                return Result.Failure(request, "New password cannot be the same as old password");
            }

            dbUser.FirstName = request.User.FirstName;
            dbUser.LastName = request.User.LastName;
            dbUser.PasswordHash = request.User.Password;
            dbUser.Pin = _secretHasher.Hash(request.User.Pin.ToString());

            UpdateUserCommand UpdateUserCommand = new() { User = dbUser };
            await new UpdateUserCommandHandler(_userManager).Handle(UpdateUserCommand, cancellationToken);

            return Result.Success(request, "Updated successfully");
        }

    }
}
