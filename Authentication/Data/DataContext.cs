using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options), IDataContext
    {
        public DbSet<User> Users { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync(new CancellationToken());
        }
        public int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
