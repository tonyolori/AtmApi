using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediatR;
using Application.Common.Models;
using Application.UserTransactions.Commands;
using Application.UserTransactions.Queries;

namespace Api.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class TransactionController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Result result = await _mediator.Send(new GetBalanceQuery { AccountNumber = accountNumber });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(uint amount)
        {
            try
            {
                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Result result = await _mediator.Send(new DepositCommand { AccountNumber = accountNumber, Amount = amount });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(uint amount)
        {
            try
            {
                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Result result = await _mediator.Send(new WithdrawCommand { AccountNumber = accountNumber, Amount = amount });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(long receivingAccount, uint amount, int pin)
        {
            try
            {
                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Result result = await _mediator.Send(new TransferCommand{
                    SendingAccount = accountNumber,
                    ReceivingAccount = receivingAccount,
                    Amount = amount,
                    Pin = pin 
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changePin")]
        public async Task<IActionResult> ChangePin(int pin)
        {
            try
            {
                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                Result result = await _mediator.Send(new ChangePinCommand { AccountNumber = accountNumber, NewPin = pin });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}