using Application.Common.Models;
using Application.Extensions;
using Application.Interfaces;
using Application.Validator;
using Domain.Entities;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Users.Commands
{
    public class LoginCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler(IAuthService authHelper,
         UserManager<User> userManager,
         SignInManager<User> signInManager
        ) : IRequestHandler<LoginCommand, Result>
    {
        private readonly IAuthService _authHelper = authHelper;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validate the request
            ValidationResult validationResult = await request.ValidateAsync(new LoginValidator(), cancellationToken);

            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join("\n ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result.Failure<AddUserCommand>(errorMessages);
            }

            User? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure(request, "User not found");
            }
            SignInResult result = await _signInManager.PasswordSignInAsync(user,
            request.Password, isPersistent: false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                // Check for specific failure reasons
                if (result.IsLockedOut)
                {
                    return Result.Failure(request, "Your account is locked out. Please try again later.");
                }
                else if (result.IsNotAllowed)
                {
                    return Result.Failure(request, "Your account is not allowed to login at this time.");
                }
                else
                {
                    // Blanket statement for other failures (e.g., invalid credentials)
                    return Result.Failure(request, "Invalid Email or password.");
                }
            }
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> authClaims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.AccountNumber.ToString()),
                    new(ClaimTypes.Name, user.FirstName),
                    new(ClaimTypes.Surname, user.LastName),
                    new(ClaimTypes.Email, user.Email),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (string userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string token = _authHelper.GenerateJWTToken(authClaims);

            return Result.Success(request, token);

        }

    }
}

