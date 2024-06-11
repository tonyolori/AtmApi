using Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Interfaces
{
    public interface IDataContext
    {
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
