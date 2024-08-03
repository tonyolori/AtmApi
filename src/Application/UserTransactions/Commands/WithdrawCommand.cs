using Application.Common.Models;
using Application.Extensions;
using Application.Users.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserTransactions.Commands;
public class WithdrawCommand : IRequest<Result>
{
    public long AccountNumber { get; set; }
    public uint Amount { get; set; }
}

public class WithdrawCommandHandler(UserManager<User> userManager) : IRequestHandler<WithdrawCommand, Result>
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Result> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userManager.FindByAccountNumber(request.AccountNumber);

        if (user == null)
        {
            return Result.Failure(request, "User not found");
        }

        if (!user.Withdraw(request.Amount))
        {
            return Result.Failure(request, "Insufficient Funds");
        }

        await new UpdateUserCommandHandler(_userManager).Handle(new UpdateUserCommand { User = user }, cancellationToken);

        return Result.Success(request, "withdraw Successful");
    }
}
