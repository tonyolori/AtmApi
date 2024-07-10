using Application.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Transactions.Queries
{
    public class GetTransactionByIdQuery(int transactionId) : IRequest<Transaction>
    {
        public int TransactionId { get; set; } = transactionId;
    }

    public class GetTransactionByIdQueryHandler(IDataContext context) : IRequestHandler<GetTransactionByIdQuery, Transaction>
    {
        private readonly IDataContext _context = context;

        public async Task<Transaction?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken: cancellationToken);
        }
    }
}
