using MediatR;
using Domain.Models;
using Application.Interfaces;

namespace Application.Transactions.Commands
{
    public class AddTransactionCommand : IRequest<string>
    {
        public Transaction Transaction { get; set; }

        public AddTransactionCommand(Transaction transaction)
        {
            Transaction = transaction;
        }
    }

    public class AddTransactionCommandHandler(IDataContext context) : IRequestHandler<AddTransactionCommand, string>
    {
        private readonly IDataContext _context = context;

        public async Task<string> Handle(AddTransactionCommand request, CancellationToken cancellationToken)
        {
            await _context.Transactions.AddAsync(request.Transaction, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);


            return "Transaction created";
        }


    }


}

