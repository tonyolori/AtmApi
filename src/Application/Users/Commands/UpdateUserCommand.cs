using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands
{
    public class UpdateUserCommand : IRequest<Result>
    {
        public User User { get; set; }
    }

    public class UpdateUserCommandHandler(UserManager<User> userManager) : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly UserManager<User> _userManager = userManager;
        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            request.User.LastModifiedDate = DateTime.Now;
            IdentityResult result = await _userManager.UpdateAsync(request.User);

            if (result.Succeeded)
            {
                return Result.Success(request, result);
            }
            string errorMessage = string.Join("\n", result.Errors.Select(e => e.Description));
            return Result.Failure(request, $"User update failed: {errorMessage}");
        }

    }

}
