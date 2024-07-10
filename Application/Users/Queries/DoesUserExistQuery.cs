using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries;

public class DoesUserExistQuery(long accountNumber) : IRequest<bool>
{
    public long AccountNumber { get; set; } = accountNumber;
}

public class DoesUserExistQueryHandler(IDataContext context) : IRequestHandler<DoesUserExistQuery, bool>
{
    private readonly IDataContext _context = context;

    public async Task<bool> Handle(DoesUserExistQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users.AnyAsync(u => u.AccountNumber == request.AccountNumber, cancellationToken: cancellationToken);
    }
}


