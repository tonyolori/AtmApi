using MediatR;
using Domain.Models;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Application.Transactions.Queries;
public class GetTransactionsQuery : IRequest<List<Transaction>>
{
}

public class GetTransactionsQueryHandler(IDataContext context) : IRequestHandler<GetTransactionsQuery, List<Transaction>>
{
    private readonly IDataContext _context = context;

    public async Task<List<Transaction>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Transactions.ToListAsync(cancellationToken: cancellationToken);
    }
}


