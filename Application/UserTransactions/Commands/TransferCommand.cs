using Application.Common.Models;
using Application.Extensions;
using Application.Interfaces;
using Application.Transactions.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Enum;
using Domain.Models;
using MediatR;

namespace Application.UserTransactions.Commands;
public class TransferCommand(long sendingAccount, long receivingAccount, uint amount, int pin) : IRequest<Result>
{
    public long SendingAccount { get; set; } = sendingAccount;
    public long ReceivingAccount { get; set; } = receivingAccount;
    public uint Amount { get; set; } =amount;
    public int Pin { get; set; } = pin;
}

public class TransferHandler(IDataContext context) : IRequestHandler<TransferCommand, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        Result result = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery(request.SendingAccount), cancellationToken);
        User sendingUser = (User)result.Entity;

        if (sendingUser == null || !sendingUser.MatchPin(request.Pin))
        {
            return Result.Failure<string>("Invalid sending user or Invalid Pin");
        }

        if (sendingUser.AccountNumber == request.ReceivingAccount)
        {
            return Result.Failure<string>("Cannot Transfer to the same account");
        }

        result = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery(request.ReceivingAccount), cancellationToken);
        User receivingUser = (User)result.Entity;

        if (receivingUser == null)
        {
            return Result.Failure<string>("Invalid Receiving user");
        }

        if (Transfer(sendingUser, receivingUser, request.Amount))
        {
            await new UpdateUserCommandHandler(_context).Handle(new UpdateUserCommand(sendingUser), cancellationToken);
            await new UpdateUserCommandHandler(_context).Handle(new UpdateUserCommand(receivingUser), cancellationToken);

            var depositTransaction = TransactionType.Deposit.CreateTransaction(request.ReceivingAccount, request.Amount, true);
            await _context.Transactions.AddAsync(depositTransaction, cancellationToken);

            var transaction = TransactionType.Transfer.CreateTransaction(request.SendingAccount,request.ReceivingAccount, request.Amount, true);
            await new AddTransactionCommandHandler(_context).Handle(new AddTransactionCommand(transaction), cancellationToken);
            
            return Result.Success("Transfer");
        }
        else
        {
            return Result.Failure<string>("insufficient funds");
        }

    }
    private bool Transfer(User
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
