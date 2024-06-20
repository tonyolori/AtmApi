using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options), IDataContext
    {
        public DbSet<User> Users { get; set; }

        public Task<int> SaveChangesAsync()
        {   
            return base.SaveChangesAsync(new CancellationToken());
        }
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
