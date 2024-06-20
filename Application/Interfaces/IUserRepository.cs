using Domain.Models;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync();
        Task<User?> GetUserAsync(int id);
        void AddUser(User user);

        void UpdateUserAsync(User user);
        void DeleteUser(User user);

        void SaveChanges();
        Task<User> GetUserByEmailAsync(string email);
        Task<User?> GetUserByAccountNumberAsync(long accountNumber);

        Task<bool> DoesUserIdExistAsync(long accountNumber);
        Task<List<long>> GetAllAdminsAsync();

        Task<bool> ValidateCredentials(string Email, string Password);

    }
}
