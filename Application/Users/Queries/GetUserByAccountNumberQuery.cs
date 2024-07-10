using MediatR;
using Domain.Models;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Common.Models;

namespace Application.Users.Queries;


public class GetUserByAccountNumberQuery(long accountNumber) : IRequest<Result>
{
    public long AccountNumber { get; set; } = accountNumber;
}

public class GetUserByAccountNumberQueryHandler(IDataContext context) : IRequestHandler<GetUserByAccountNumberQuery, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(GetUserByAccountNumberQuery request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
        .FirstOrDefaultAsync(u => u.AccountNumber == request.AccountNumber, cancellationToken: cancellationToken);
        if (user == null)
        {
            return Result.Failure<string>("User not found.");
        }

        return Result.Success(user);
    }
}
