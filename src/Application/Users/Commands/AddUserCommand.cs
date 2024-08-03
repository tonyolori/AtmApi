using Application.Common.Models;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public class AddUserCommand : IRequest<Result>
{
    public UserDto User { get; set; }
    public UserRole UserRole { get; set; }
}

public class AddUserCommandHandler(
            IEmailService emailSender,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ISecretHasherService secretHasher)
 : IRequestHandler<AddUserCommand, Result>
{
    private readonly IEmailService _emailSender = emailSender;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly ISecretHasherService _secretHasher = secretHasher;

    public async Task<Result> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        User? userExists = await _userManager.FindByEmailAsync(request.User.Email);

        if (userExists != null)
            return Result.Failure(request, "User Already Exists");

        User user = new()
        {
            AccountNumber = GenerateRandomAccountNumber(),
            Balance = 0,
            Email = request.User.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            FirstName = request.User.FirstName,
            UserName = request.User.FirstName + request.User.LastName,
            LastName = request.User.LastName,
            Pin = _secretHasher.Hash(request.User.Pin.ToString()),
            Status = Status.Active,
            StatusDesc = Status.Active.ToString(),
            CreatedDate = DateTime.Now,
            LastModifiedDate = DateTime.Now,
        };

        //it has its inbuilt validator
        IdentityResult result = await _userManager.CreateAsync(user, request.User.Password);

        if (!result.Succeeded)
        {
            string errors = string.Join("\n", result.Errors.Select(e => e.Description));
            return Result.Failure(request, "\"User creation failed!\n" + errors);
        }


        if (!await _roleManager.RoleExistsAsync(request.UserRole.ToString()))
            await _roleManager.CreateAsync(new IdentityRole(request.UserRole.ToString()));

        if (await _roleManager.RoleExistsAsync(request.UserRole.ToString()))
        {
            await _userManager.AddToRoleAsync(user, request.UserRole.ToString());
        }


        await _emailSender.SendEmailAsync(request.User.Email, request.User.FirstName);
        return Result.Success("User created successfully!", user, request);

    }

    private static long GenerateRandomAccountNumber()
    {
        Random random = new();
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
            exists = false; //TODO: find by account number
        } while (exists);

        return await Task.FromResult(newId);
    }

}
