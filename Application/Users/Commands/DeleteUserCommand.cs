using MediatR;
using Domain.Models;
using Application.Interfaces;
using Application.Common.Models;
using Application.Users.Queries;

namespace Application.Users.Commands
{
    public class DeleteUserCommand(long accountNumber) : IRequest<Result>
    {
        public long AccountNumber { get; set; } = accountNumber;
    }

    public class DeleteUserCommandHandler(IDataContext context) : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IDataContext _context = context;

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            Result result = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery(request.AccountNumber), cancellationToken);
            User user = (User)result.Entity;

            if (user == null)
            {
                return Result.Failure<string>("Invalid User");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Result.Success("User Deleted");
        }

    }


}

