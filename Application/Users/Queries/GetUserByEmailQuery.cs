using MediatR;
using Domain.Models;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Common.Models;

namespace Application.Users.Queries;


public class GetUserByEmailQuery(string email) : IRequest<Result>
{
    public string Email { get; set; } = email;
}

public class GetUserByEmailQueryHandler(IDataContext context) : IRequestHandler<GetUserByEmailQuery, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken: cancellationToken);
        if (user == null)
        {
            return Result.Failure<string>("User not found");
        }

        return Result.Success(user);
    }
}
