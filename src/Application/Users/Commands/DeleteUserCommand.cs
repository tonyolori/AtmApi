using Application.Common.Models;
using Application.Extensions;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;
public class DeleteUserCommand : IRequest<Result>
{
    public long? AccountNumber { get; set; }
    public string? Email { get; set; }

}

public class DeleteUserCommandHandler(UserManager<User> userManager) : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        User? user = request.AccountNumber != null
            ? await _userManager.FindByAccountNumber((long)request.AccountNumber)
            : await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            //TODO: can you propagate this
            return Result.Failure(request, "Invalid User");
        }

        user.Status = Status.Inactive;
        user.StatusDesc = Status.Inactive.ToString();

        return Result.Success(request, "User Deleted");

    }

}



