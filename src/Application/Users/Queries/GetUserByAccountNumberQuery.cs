using Application.Common.Models;
using Application.Extensions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
namespace Application.Users.Queries;


public class GetUserByAccountNumberQuery : IRequest<Result>
{
    public long AccountNumber { get; set; }
}

public class GetUserByAccountNumberQueryHandler(UserManager<User> userManager) : IRequestHandler<GetUserByAccountNumberQuery, Result>
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Result> Handle(GetUserByAccountNumberQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userManager.FindByAccountNumber(request.AccountNumber);

        if (user == null)
        {
            return Result.Failure(request, "User not found.");
        }

        return Result.Success(user, request);
    }
}
