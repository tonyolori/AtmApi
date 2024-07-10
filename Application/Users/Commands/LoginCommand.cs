using MediatR;
using Domain.Models;
using Application.Interfaces;
using Application.Common.Models;
using Application.Users.Queries;
using Application.Common.DTOs;
using Application.Common;
using Application.Validator;
using FluentValidation.Results;

namespace Application.Users.Commands
{
    public class LoginCommand(LoginDto loginDto) : IRequest<Result>
    {
        public LoginDto LoginDto = loginDto;
    }

    public class LoginCommandHandler(IDataContext context,IAuthHelper authHelper, ISecretHasher secretHasher) : IRequestHandler<LoginCommand, Result>
    {
        private readonly IDataContext _context = context;
        private readonly IAuthHelper _authHelper = authHelper;
        private readonly ISecretHasher _secretHasher = secretHasher;

        public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate input (same as before)
            LoginValidator validator = new();
            ValidationResult valResult = validator.Validate(request.LoginDto);

            if (!valResult.IsValid)
            {
                string errors = string.Join("\n", Utils.GetPrintableErrorList(valResult.Errors));
                return Result.Failure<string>(errors);
            }


            var GetUserByEmailQuery = new GetUserByEmailQuery(request.LoginDto.Email);
            Result result = await new GetUserByEmailQueryHandler(_context).Handle(GetUserByEmailQuery, cancellationToken);
            
            User? storedUser = (User?)result.Entity;

            if (storedUser == null)
            {
                return Result.Failure<string>("User does not exist");
            }

            if(_secretHasher.Verify(request.LoginDto.Password,storedUser.Password))
            {
                var token = _authHelper.GenerateJWTToken(storedUser);
                return Result.Success<string>(token);
            }

            return Result.Failure<string>("Invalid Email or Password");

        }

    }
}

