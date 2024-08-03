using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public class UnlockUserCommand : IRequest<Result>
{
    public string Email { get; set; }
}

public class UnlockUserCommandHandler(UserManager<User> userManager) : IRequestHandler<UnlockUserCommand, Result>
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Result> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Result.Failure(request, "User not found.");
        }

        // Reset the access failed count and unlock the user
        IdentityResult resetAccessFailedCountResult = await _userManager.ResetAccessFailedCountAsync(user);
        if (!resetAccessFailedCountResult.Succeeded)
        {
            return Result.Failure(request, "Failed to reset access failed count.");
        }

        IdentityResult unlockUserResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);
        if (!unlockUserResult.Succeeded)
        {
            return Result.Failure(request, "Failed to unlock the user.");
        }

        return Result.Success(request, "User account has been unlocked successfully.");
    }
}
