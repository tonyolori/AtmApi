using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Users.Queries;
using Domain.Entities;
using Domain.Enum;
using Application.UserTransactions.Commands;
using Application.Common.Models;
using Application.UserTransactions.Queries;
using Application.Users.Commands;
using Microsoft.AspNetCore.Authorization;

namespace AtmApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount(UserDto account)
    {
        AddUserCommand command = new() { User = account, UserRole = UserRole.Admin };
        Result result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers(int? role, string? searchValue)
    {
        FilterUsersQuery query = new() { UserRole = role, SearchValue = searchValue };
        Result result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("balance/{accountNumber}")]

    public async Task<IActionResult> GetBalance(long accountNumber)
    {
        GetBalanceQuery query = new() { AccountNumber = accountNumber };
        Result result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("unlock")]
    public async Task<ActionResult> UnlockUser(UnlockUserCommand unlockUserCommand)
    {
        Result result = await _mediator.Send(unlockUserCommand);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(DepositCommand depositCommand)
    {
        Result result = await _mediator.Send(depositCommand);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(WithdrawCommand withdrawCommand)
    {
        Result result = await _mediator.Send(withdrawCommand);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(TransferCommand transferCommand)
    {

        Result result = await _mediator.Send(transferCommand);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("changePin")]
    public async Task<ActionResult<User>> ChangePin(ChangePinCommand changePinCommand)
    {
        Result result = await _mediator.Send(changePinCommand);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> DeleteUser(DeleteUserCommand deleteUserCommand)
    {
        Result result = await _mediator.Send(deleteUserCommand);
        return Ok(result);
    }

}