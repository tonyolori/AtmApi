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

public class DepositCommand(long accountNumber,uint amount) : IRequest<Result>
{
    public long AccountNumber { get; set; } = accountNumber;
    public uint Amount { get; set; } = amount;
}



public class DepositHandler(IDataContext context) : IRequestHandler<DepositCommand, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        Result result = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery(request.AccountNumber), cancellationToken);
        User user = (User)result.Entity;

        if (user == null)
        {
            return Result.Failure<string>("user not found");
        }

        user.Deposit(request.Amount);
        await new UpdateUserCommandHandler(_context).Handle(new UpdateUserCommand(user), cancellationToken);


        var transaction = TransactionType.Deposit.CreateTransaction(request.AccountNumber, request.Amount, true);
        await new AddTransactionCommandHandler(_context).Handle(new AddTransactionCommand(transaction), cancellationToken);

        return Result.Success<string>("ok");
    }
}
