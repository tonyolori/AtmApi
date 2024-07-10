using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MediatR;
using Application.UserTransactions.Queries;
using Application.UserTransactions.Commands;
using Application.Common.Models;

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
                Result result = await _mediator.Send(new GetBalanceQuery(accountNumber));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(uint amount, int pin)
        {
            try
            {
                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Result result = await _mediator.Send(new DepositCommand(accountNumber, amount));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(uint amount, int pin)
        {
            try
            {
            long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Result result = await _mediator.Send(new WithdrawCommand(accountNumber, amount));
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
            var result = await _mediator.Send(new TransferCommand(accountNumber, receivingAccount, amount, pin));
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
            Result result = await _mediator.Send(new ChangePinCommand(accountNumber, pin));
            return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
















//namespace AtmApi.Controllers
//{
//    [Route("api/")]
//    [ApiController]
//    [Authorize]

//    public class TransactionController(IUserRepository userRepository,
//        TransactionHelper transactionManager, ITransactionRepository transactionRepository) : Controller
//    {
//        private readonly IUserRepository _userRepository = userRepository;
//        private readonly ITransactionRepository _transactionRepository = transactionRepository;
//        private readonly TransactionHelper _transactionManager = transactionManager;

//        [HttpGet("balance")]

//        public async Task<IActionResult> GetBalance()
//        {
//            User? user;
//            long accountNumber;
//            try
//            {
//                accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

//                user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);
//                if (user == null)
//                {
//                    return NotFound();
//                }
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }

//            long balance = user.GetBalance();

//            var transaction = TransactionType.BalanceQuery.CreateTransaction(accountNumber, balance, true);
//            await _transactionRepository.AddTransaction(transaction);

//            return Ok(balance);//pass back a balance response or text
//        }

//        [HttpPost("deposit")]
//        public async Task<IActionResult> Deposit(uint amount, int pin)
//        {
//            User? user;
//            long accountNumber;
//            try
//            {
//                accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

//                user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);


//                if (user == null)
//                {
//                    return NotFound();
//                }

//                if (!user.MatchPin(pin))
//                {
//                    return BadRequest("Invalid Pin");
//                }


//                user.Deposit(amount);
//                _userRepository.UpdateUserAsync(user);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }


//            var transaction = TransactionType.Deposit.CreateTransaction(accountNumber, amount, true);
//            await _transactionRepository.AddTransaction(transaction);
//            return Ok();
//        }

//        [HttpPost("withdraw")]
//        public async Task<IActionResult> Withdraw(uint amount, int pin)
//        {

//            User? user;
//            try
//            {
//                long accountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

//                user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);
//                if (user == null)
//                {
//                    return NotFound();
//                }
//                else if (!user.MatchPin(pin))
//                {
//                    return BadRequest("Invalid Pin");
//                }

//                user.Withdraw(amount);
//                var transaction = TransactionType.Withdrawal.CreateTransaction(accountNumber, amount, true);
//                await _transactionRepository.AddTransaction(transaction);
//                _userRepository.UpdateUserAsync(user);

//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }

//            return Ok();
//        }

//        [HttpPost("transfer")]
//        public async Task<IActionResult> Transfer(long receivingAccount, uint amount, int pin)
//        {

//            bool _ = long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out long accountNumber);

//            User? sendingUser = await _userRepository.GetUserByAccountNumberAsync(accountNumber);

//            if (sendingUser == null || !sendingUser.MatchPin(pin))
//            {
//                return BadRequest("Invalid sending user or Invalid Pin");
//            }
//            if (sendingUser.AccountNumber == receivingAccount)
//            {
//                return BadRequest("Cannot Transfer to the same account");
//            }



//            User? ReceivingUser = await _userRepository.GetUserByAccountNumberAsync(receivingAccount);
//            if (ReceivingUser == null)
//            {
//                return BadRequest("Invalid Receiving user");
//            }

//            if (_transactionManager.Transfer(sendingUser, ReceivingUser, amount))
//            {
//                var transaction = TransactionType.Deposit.CreateTransaction(accountNumber, amount, true);
//                await _transactionRepository.AddTransaction(transaction);
//                return Ok(CustomResponseMessage.Sucess("Transfer"));
//            }
//            else
//            {
//                return BadRequest();
//            }

//        }

//        [HttpPost("changePin")]
//        public async Task<ActionResult<User>> ChangePin(int pin)
//        {

//            User? user;

//            try
//            {
//                long uAccountNumber = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

//                user = await _userRepository.GetUserByAccountNumberAsync(uAccountNumber);
//                if (user == null)
//                {
//                    return NotFound();
//                }

//                if (user.MatchPin(pin))
//                {
//                    return BadRequest("Pin cannot be the same as old pin");
//                }

//                _transactionManager.ChangePin(user, pin);
//            }
//            catch (Exception e)
//            {
//                return BadRequest(e.Message);

//            }

//            return Ok(user);
//        }



//    }
//}
