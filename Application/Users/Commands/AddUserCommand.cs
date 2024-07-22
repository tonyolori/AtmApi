using MediatR;
using Domain.Models;
using Application.Interfaces;
using Application.Validator;
using Domain.Enum;
using FluentValidation.Results;
using Application.Common.Models;
using Application.Common;
using Application.Users.Queries;

namespace Application.Users.Commands;

public class AddUserCommand(UserDto user, UserRole role) : IRequest<Result>
{
    public UserDto User { get; set; } = user;
    public UserRole UserRole { get; set; } = role;
}

public class AddUserCommandHandler(IDataContext context, ISecretHasher secretHasher, IEmailSender emailSender) : IRequestHandler<AddUserCommand, Result>
{
    private readonly IDataContext _context = context;
    private readonly ISecretHasher _secretHasher = secretHasher;
    private readonly IEmailSender _emailSender = emailSender;
    public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        // Validate input (same as before)
        UserValidator validator = new();
        ValidationResult result = validator.Validate(request.User);

        if (!result.IsValid)
        {
            return Result.Failure<string>(Utils.GetPrintableErrorString(result.Errors));
        }

        //confirm the email is not in the database
        Result dbUserResult = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery(request.User.Email), cancellationToken);
        User? dbUser = (User)dbUserResult.Entity;

        if (dbUser != null)
        {
            return Result.Failure<string>("User Already exists");
        }

        // Create user command
        User user = new()
        {
            FirstName = request.User.FirstName,
            LastName = request.User.LastName,
            AccountNumber = await GenerateUniqueAccount(cancellationToken),//test generator
            Email = request.User.Email,
            Balance = 0,
            Pin = request.User.Pin,
            Password = _secretHasher.Hash(request.User.Password),
            Role = request.UserRole,
        };




        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _emailSender.SendEmailAsync(request.User.Email, request.User.FirstName);
        return Result.Success(user);
    }

    private static long GenerateRandomAccountNumber()
    {
        var random = new Random();
        long accountId = 0;


        for (int i = 0; i < 10; i++)
        {
            accountId = accountId * 10 + random.Next(1, 10); // Generate a random digit (0-9) and add it to the account number
        }

        return accountId;
    }

    public async Task<long> GenerateUniqueAccount(CancellationToken cancellationToken)
    {
        long newId;
        bool exists;

        do
        {
            newId = GenerateRandomAccountNumber();
            exists = await new DoesUserExistQueryHandler(_context).Handle(new DoesUserExistQuery(newId), cancellationToken);
        } while (exists);

        return newId;
    }

}








        


