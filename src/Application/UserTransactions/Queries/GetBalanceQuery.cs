using Application.Common.Models;
using Application.Extensions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UserTransactions.Queries;

public class GetBalanceQuery : IRequest<Result>
{
    public long AccountNumber { get; set; }
}



public class GetBalanceHandler(UserManager<User> userManager) : IRequestHandler<GetBalanceQuery, Result>
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Result> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userManager.FindByAccountNumber(request.AccountNumber);

        if (user == null)
        {
            return Result.Failure<string>("User not found");
        }

        long balance = user.GetBalance();

        return Result.Success(balance);
    }
}
