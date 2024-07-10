using Domain.Models;

namespace Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetTransactionsAsync();
        Task<Transaction?> GetTransactionAsync(int id);
        Task AddTransaction(Transaction transaction);

        Task DeleteTransaction(Transaction transaction);

    }
}
