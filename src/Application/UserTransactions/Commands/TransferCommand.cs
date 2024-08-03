using Application.Common.Models;
using Application.Extensions;
using Application.Interfaces;
using Application.Users.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserTransactions.Commands;

public class TransferCommand : IRequest<Result>
{
    public long SendingAccount { get; set; }
    public long ReceivingAccount { get; set; }
    public uint Amount { get; set; }
    public int Pin { get; set; }
}

public class TransferHandler(UserManager<User> userManager, ISecretHasherService secretHasher) : IRequestHandler<TransferCommand, Result>
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly ISecretHasherService _secretHasher = secretHasher;

    public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        User? sendingUser = await _userManager.FindByAccountNumber(request.SendingAccount);
        string hashedSenderPin = _secretHasher.Hash(request.Pin.ToString());

        if (sendingUser == null || !sendingUser.MatchPin(hashedSenderPin))
        {
            return Result.Failure(request, "Invalid sending user or Invalid Pin");
        }

        if (sendingUser.AccountNumber == request.ReceivingAccount)
        {
            return Result.Failure(request, "Cannot Transfer to the same account");
        }

        User? receivingUser = await _userManager.FindByAccountNumber(request.ReceivingAccount);

        if (receivingUser == null)
        {
            return Result.Failure(request, "Invalid Receiving user");
        }

        if (Transfer(sendingUser, receivingUser, request.Amount))
        {
            await new UpdateUserCommandHandler(_userManager).Handle(new UpdateUserCommand { User = sendingUser }, cancellationToken);
            await new UpdateUserCommandHandler(_userManager).Handle(new UpdateUserCommand { User = receivingUser }, cancellationToken);

            return Result.Success("Transfer Success");
        }
        else
        {
            return Result.Failure(request, "insufficient funds");
        }

    }
    private static bool Transfer(User
            sendingAccount, User
            receivingAccount, uint amount)
    {
        if (sendingAccount.Withdraw(amount))
        {
            receivingAccount.Deposit(amount);
        }
        else
        {
            return false;
        }


        return true;
    }
}
