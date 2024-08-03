using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions;
public static class UserManagerExtensions
{
    public static async Task<User?> FindByAccountNumber(this UserManager<User> manager, long accountNumber)
    {
        return await manager.Users.SingleOrDefaultAsync(x => x.AccountNumber == accountNumber);
    }
}

