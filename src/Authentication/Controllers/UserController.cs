using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.Users.Commands;
using Application.Common.Models;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Application.UserTransactions.Commands;
using System.Security.Claims;


namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMediator mediator, UserManager<User> userManager, SignInManager<User> signInManager) : ControllerBase
    {

        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        private readonly IMediator _mediator = mediator;

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto account)
        {
            AddUserCommand command = new() { User = account, UserRole = UserRole.User };
            Result result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginCommand loginCommand)
        {
            Result result = await _mediator.Send(loginCommand);
            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUser(ChangeNameAndPasswordCommand changeUserNameAndPasswordCommand)
        {
            Result result = await _mediator.Send(changeUserNameAndPasswordCommand);
            return Ok(result);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteUser()
        {
            long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            DeleteUserCommand DeleteUserCommand = new() { AccountNumber = accountNumber };
            Result result = await _mediator.Send(DeleteUserCommand);
            return Ok(result);
        }

    }

}
