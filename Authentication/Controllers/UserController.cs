using Application.ActionFilters;
using Domain.Enum;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.Users.Commands;
using Application.Common.Models;


namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto user)
        {
            try
            {
                var AddUserCommand = new AddUserCommand(user, UserRole.User);
                Result result = await _mediator.Send(AddUserCommand);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        [ValidateEmail]
        [ValidatePassword]
        public async Task<ActionResult> Login(string Email, string Password)
        {
            try
            {
                var LoginCommand = new LoginCommand(Email, Password);
                Result result = await _mediator.Send(LoginCommand);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateUser(UserDto user)
        {
            try
            {
                var changeUserNameAndPasswordCommand = new ChangeUserNameAndPasswordCommand(user);
                Result result = await _mediator.Send(changeUserNameAndPasswordCommand);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteUser()
        {
            try
            {
                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var DeleteUserCommand = new DeleteUserCommand(accountNumber);
                Result result = await _mediator.Send(DeleteUserCommand);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }

}
