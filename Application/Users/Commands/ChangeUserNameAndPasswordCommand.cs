using Application.Common;
using Application.Common.Models;
using Application.Interfaces;
using Application.Users.Queries;
using Application.Validator;
using Domain.Models;
using FluentValidation.Results;
using MediatR;

namespace Application.Users.Commands
{
    public class ChangeUserNameAndPasswordCommand(UserDto user) :IRequest<Result>
    {
        public UserDto User { get; set; } = user;
    }

    public class ChangeUserNameAndPasswordCommandHandler(IDataContext context,ISecretHasher secretHasher) : IRequestHandler<ChangeUserNameAndPasswordCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly ISecretHasher _secretHasher = secretHasher;
        public async Task<Result> Handle(ChangeUserNameAndPasswordCommand request, CancellationToken cancellationToken)
        {
            UserValidator validator = new();

            ValidationResult result = validator.Validate(request.User);

            if (!result.IsValid)
            {
                return Result.Failure<string>(Utils.GetPrintableErrorList(result.Errors));
            }

            Result dbUserResult = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery(request.User.Email), cancellationToken);
            User? dbUser = (User)dbUserResult.Entity;

            if (dbUser == null)
            {
                return Result.Failure<string>("user not found");
            }

            string newPassword = _secretHasher.Hash(request.User.Password);
            if(newPassword == dbUser.Password)
            {
                return Result.Failure<string>("New password cannot be the same as old password");
            }
            
            dbUser.FirstName = request.User.FirstName;
            dbUser.LastName = request.User.LastName;
            dbUser.Password = request.User.Password;


            var UpdateUserCommand = new UpdateUserCommand(dbUser);
            await new UpdateUserCommandHandler(context).Handle(UpdateUserCommand, cancellationToken);

            return Result.Success("Updated successfully");
        }

    }
}
