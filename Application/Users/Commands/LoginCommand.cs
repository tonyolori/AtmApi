using MediatR;
using Domain.Models;
using Application.Interfaces;
using Application.Common.Models;
using Application.Helpers;
using Application.Users.Queries;

namespace Application.Users.Commands
{
    public class LoginCommand(string email, string password) : IRequest<Result>
    {
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;

    }

    public class LoginCommandHandler(IDataContext context,AuthHelper authHelper) : IRequestHandler<LoginCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly AuthHelper _authHelper = authHelper;

        public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var GetUserByEmailQuery = new GetUserByEmailQuery(request.Email);
            var result = await new GetUserByEmailQueryHandler(context).Handle(GetUserByEmailQuery, cancellationToken);
            User? storedUser = (User?)result.Entity;

            if (storedUser == null)
            {
                return Result.Failure<string>("User does not exist");
            }

            if(request.Password == storedUser.Password)
            {
                var token = _authHelper.GenerateJWTToken(storedUser);
                return Result.Success<string>(token);
            }

            return Result.Failure<string>("Invalid Email or Password");

        }

    }
}

