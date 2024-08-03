using Application.Common.Models;
using Application.Extensions;
using Application.Interfaces;
using Application.Users.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserTransactions.Commands
{
    public class ChangePinCommand : IRequest<Result>
    {
        public long AccountNumber { get; set; }
        public string OldPin { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangePinCommandHandler(
        ISecretHasherService secretHasher,
        UserManager<User> userManager
        ) : IRequestHandler<ChangePinCommand, Result>
    {
        private readonly ISecretHasherService _secretHasher = secretHasher;
        private readonly UserManager<User> _userManager = userManager;

        public async Task<Result> Handle(ChangePinCommand request, CancellationToken cancellationToken)
        {

            User? dbUser = await _userManager.FindByAccountNumber(request.AccountNumber);

            if (dbUser == null)
            {
                return Result.Failure(request, "user not found");
            }

            string hashedNewPin = _secretHasher.Hash(request.NewPin.ToString());

            if (request.OldPin != dbUser.Pin)
            {
                return Result.Failure(request, "Invalid Pin");

            }

            if (hashedNewPin.Equals(request.OldPin))
            {
                return Result.Failure(request, "New password cannot be the same as old password");
            }

            dbUser.Pin = hashedNewPin;

            UpdateUserCommand UpdateUserCommand = new() { User = dbUser };
            await new UpdateUserCommandHandler(_userManager).Handle(UpdateUserCommand, cancellationToken);

            return Result.Success(request, "Updated successfully");
        }

    }
}
