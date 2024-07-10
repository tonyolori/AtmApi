using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;
public interface IDataContext
{
    DbSet<User> Users { get; set; }
    DbSet<Transaction> Transactions { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellation);

    int SaveChanges();
}
