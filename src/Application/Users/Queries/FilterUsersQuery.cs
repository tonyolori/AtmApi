using Application.Common.Models;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web.WebPages;

namespace Application.Users.Queries;

public class FilterUsersQuery : IRequest<Result>
{
    public string? SearchValue { get; set; }
    public int? UserRole { get; set; }
}

public class FilterUsersQueryHandler(UserManager<User> userManager) : IRequestHandler<FilterUsersQuery, Result>
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Result> Handle(FilterUsersQuery request, CancellationToken cancellationToken)
    {

        List<User> users = await _userManager.Users.ToListAsync(cancellationToken: cancellationToken);
        if (request.UserRole != null)
        {
            //invalid role
            if (request.UserRole < 0 || request.UserRole > 2)
            {
                return Result.Failure(request, "Invalid UserRole");
            }
            users = FilterByRole(users, GetUserRoleString((int)request.UserRole), _userManager);
        }

        if (!request.SearchValue.IsEmpty())
        {
            users = FilterByName(users, request.SearchValue);
        }

        return Result.Success(users, request);

    }

    private static List<User> FilterByRole(List<User> users, string userRole, UserManager<User> _userManager)
    {
        return users.Where(user =>
        {
            IList<string> userRoles = _userManager.GetRolesAsync(user).Result;
            return userRoles.Contains(userRole);
        }).ToList();
    }

    private static string GetUserRoleString(int userRoleInt)
    {
        if (Enum.IsDefined(typeof(UserRole), userRoleInt))
        {
            return ((UserRole)userRoleInt).ToString();
        }
        else
        {
            // Handle invalid integer value (e.g., return a default value or throw an exception)
            return "Unknown";
        }
    }

    private static List<User> FilterByName(List<User> users, string searchValue)
    {
        return users.Where(u => u.FirstName.Contains(searchValue) || u.LastName.Contains(searchValue)).ToList();
    }
}


