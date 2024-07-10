using Application.Common.Models;
using Application.Helpers;
using Application.Interfaces;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Models;
using MediatR;

namespace Application.UserTransactions.Commands;

public class ChangePinCommand(long accountNumber, int newPin) : IRequest<Result>
{
    public long AccountNumber { get; set; } = accountNumber;
    public int NewPin { get; set; } = newPin;
}

public class ChangePinHandler(IDataContext context) : IRequestHandler<ChangePinCommand, Result>
{
    private readonly IDataContext _context = context;

    public async Task<Result> Handle(ChangePinCommand request, CancellationToken cancellationToken)
    {
        Result result = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery(request.AccountNumber), cancellationToken);
        User user = (User)result.Entity;

        if (user == null)
        {
            return Result.Failure<string>("User not found");
        }

        if (user.MatchPin(request.NewPin))
        {
            return Result.Failure<string>("New Pin Cannot be the same as old pin");
        }

        if (request.NewPin.ToString().Length != 4)
        {
            return Result.Failure<string>("Pin length not equal to 4");
        }

        user.ChangePin(request.NewPin);

        await new UpdateUserCommandHandler(_context).Handle(new UpdateUserCommand(user), cancellationToken);

        return Result.Success(user);
    }
}
