using Application.Common.Models;
using Application.Helpers;
using Application.Interfaces;
using Application.Transactions.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Enum;
using Domain.Models;
using MediatR;

namespace Application.UserTransactions.Commands;
public class WithdrawCommand(long accountNumber, uint amount) : IRequest<Result>
{
    public long AccountNumber { get; set; } = accountNumber;
    public uint Amount { get; set; } = amount;
}

public class WithdrawHandler(IDataContext context) : IRequestHandler<WithdrawCommand, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        Result result = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery(request.AccountNumber), cancellationToken);
        User user = (User)result.Entity;

        if (user == null)
        {
            return Result.Failure<string>("User not found");
        }

        if (!user.Withdraw(request.Amount))
        {
            return Result.Failure<string>("Insufficient Funds");
        }
        
        var transaction = TransactionType.Withdrawal.CreateTransaction(request.AccountNumber, request.Amount, true);
        await new AddTransactionCommandHandler(_context).Handle(new AddTransactionCommand(transaction), cancellationToken);

        await new UpdateUserCommandHandler(_context).Handle(new UpdateUserCommand(user), cancellationToken);

        return Result.Success(user,"Updated Successfully");
    }
}
