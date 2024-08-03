using Application.Common.Models;
using Application.Extensions;
using Application.Users.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserTransactions.Commands;

public class DepositCommand : IRequest<Result>
{
    public long AccountNumber { get; set; }
    public uint Amount { get; set; }
}



public class DepositCommandHandler(UserManager<User> userManager) : IRequestHandler<DepositCommand, Result>
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userManager.FindByAccountNumber(request.AccountNumber);

        if (user == null)
        {
            return Result.Failure(request, "user not found");
        }

        user.Deposit(request.Amount);
        await new UpdateUserCommandHandler(_userManager).Handle(new UpdateUserCommand { User = user }, cancellationToken);

        return Result.Success(request, "Deposit successful");
    }
}
