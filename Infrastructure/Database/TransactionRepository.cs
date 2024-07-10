//using Application.Interfaces;
//using Domain.Models;
//using Microsoft.EntityFrameworkCore;

//namespace Infrastructure.Database;

//public class TransactionRepository(IDataContext context) : ITransactionRepository
//{
//    private readonly IDataContext _context = context;

//    public async Task<List<Transaction>> GetTransactionsAsync()
//    {
//        return await _context.Transactions.ToListAsync();

//    }
//    public async Task<Transaction?> GetTransactionAsync(int id)
//    {
//        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
//    }

//    public async Task AddTransaction(Transaction transaction)
//    {
//        await _context.Transactions.AddAsync(transaction);
//        await _context.SaveChangesAsync();
//    }

//    public async Task DeleteTransaction(Transaction transaction)
//    {
//        _context.Transactions.Remove(transaction);
//        await _context.SaveChangesAsync();
//    }

//}

