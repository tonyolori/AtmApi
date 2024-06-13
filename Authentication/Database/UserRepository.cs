using Authentication.Data;
using Authentication.Interfaces;
using Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Database
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataContext _context;

        public UserRepository(IDataContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = await _context.Users.ToListAsync();

            return users;
        }
        public async Task<User?> GetUserAsync(int id)
        {
            User? user = await _context.Users.FindAsync(id);

            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {


            return await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
        public async Task<User?> GetUserByAccountNumberAsync(long accountNumber)
        {
            User? user = await _context.Users
                    .FirstOrDefaultAsync(u => u.AccountNumber == accountNumber);

            return user;
        }


        public async Task<List<long>> GetAllAdminsAsync()
        {
            var users = await GetUsersAsync();
            var admins = users.Where(u => u.Role == Enum.UserRole.Admin);
            List<long> adminsAccountNumbers = admins.Select(u => u.AccountNumber).ToList();

            return adminsAccountNumbers;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();

        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();

        }

        public async void SaveChanges()
        {
            await _context.SaveChangesAsync();

        }

        public async Task<bool> DoesUserIdExistAsync(long accountNumber)
        {
            return await _context.Users.AnyAsync(u => u.AccountNumber == accountNumber);
        }


    }
}
