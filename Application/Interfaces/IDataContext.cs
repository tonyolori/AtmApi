using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;
public interface IDataContext
{
    DbSet<User> Users { get; set; }
    Task<int> SaveChangesAsync();
    int SaveChanges();
}
