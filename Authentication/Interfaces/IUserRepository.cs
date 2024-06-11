using Authentication.Models;

namespace Authentication.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();
        public Task<User?> GetUserAsync(int id);
        public void AddUser(User user);

        public void UpdateUserAsync(User user);
        public void DeleteUser(User user);

        public void SaveChanges();
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User?> GetUserByAccountNumberAsync(long accountNumber);

        public Task<bool> DoesUserIdExistAsync(long accountNumber);
        public Task<List<long>> GetAllAdminsAsync();

    }
}
