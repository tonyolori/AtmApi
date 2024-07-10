using Application.Common.Models;
using Application.Helpers;
using Application.Interfaces;
using Application.Transactions.Commands;
using Application.Users.Queries;
using Domain.Enum;
using Domain.Models;
using MediatR;

namespace Application.UserTransactions.Queries;

public class GetBalanceQuery(long accountNumeber) : IRequest<Result>
{
    public long AccountNumber { get; set; } = accountNumeber;
}



public class GetBalanceHandler(IDataContext context) : IRequestHandler<GetBalanceQuery, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        Result result = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery(request.AccountNumber), cancellationToken);
        User user = (User)result.Entity;

        if (user == null)
        {
            return Result.Failure<string>("User not found");
        }

        long balance = user.GetBalance();

        var transaction = TransactionType.BalanceQuery.CreateTransaction(request.AccountNumber, balance, true);
        await new AddTransactionCommandHandler(_context).Handle(new AddTransactionCommand(transaction),cancellationToken);

        return Result.Success(balance);
    }
}
