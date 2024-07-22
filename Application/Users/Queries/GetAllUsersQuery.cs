using MediatR;
using Domain.Models;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Common.Models;
using Domain.Enum;
using Microsoft.IdentityModel.Tokens;
using System.Web.WebPages;

namespace Application.Users.Queries;

public class GetAllUsersQuery : IRequest<Result>
{
    public string SearchValue { get; set; }
    public int UserRole { get; set; }
}

public class GetUsersHandler(IDataContext context) : IRequestHandler<GetAllUsersQuery, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {

        List<User> users = await _context.Users.ToListAsync(cancellationToken: cancellationToken);
        if (request.UserRole > 0)
        {

            users = FilterByRole(users, request.UserRole);
        }

        if (!request.SearchValue.IsEmpty())
        {
            users = FilterByName(users, request.SearchValue);
        }

        return Result.Success(users);

    }

    private static List<User> FilterByRole(List<User> users, int role)
    {
        UserRole UserRole = (UserRole)role;

        switch (UserRole)
        {

            case UserRole.Admin:
                return users.Where(u => u.Role == UserRole.Admin).ToList();
            case UserRole.User:
                return users.Where(u => u.Role == UserRole.User).ToList();
            default:
                return users;
        }
    }

    private static List<User> FilterByName(List<User> users, string searchValue)
    {
        return users.Where(u => u.FirstName.Contains(searchValue) || u.LastName.Contains(searchValue)).ToList();
    }
}


