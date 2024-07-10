using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Transactions.Commands;
public class DeleteTransactionCommand(int transactionId) : IRequest
{
    public int TransactionId { get; set; } = transactionId;
}

public class DeleteTransactionCommandHandler(IDataContext context) : IRequestHandler<DeleteTransactionCommand>
{
    private readonly IDataContext _context = context;

    public async Task Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        //get the transaction record from the database using the id
        var transaction =  await _context.Transactions.FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken: cancellationToken);

        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return;
    }

}
