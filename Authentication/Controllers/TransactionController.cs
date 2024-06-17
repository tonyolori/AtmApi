using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Infrastructure.Interfaces;
using Authentication.Logic;
using Domain.Models;

namespace AtmApi.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]

    public class TransactionController(IUserRepository userRepository, 
        TransactionManager transactionManager) : Controller
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly TransactionManager _transactionManager = transactionManager;

        [HttpGet("balance")]

        public async Task <IActionResult> GetBalance()
        {
            string accountNumber = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User? user;
            try
            {
                user = await _userRepository.GetUserByAccountNumberAsync(long.Parse(accountNumber));
                if (user == null) 
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return base.Ok(user.GetBalance());//pass back a balancxe response or text
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(uint amount, int pin)
        {
            string accountNumber = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user;
            try
            {
                user = await _userRepository.GetUserByAccountNumberAsync(long.Parse(accountNumber));

                if (user == null)
                {
                    return NotFound();
                }

                if (!user.MatchPin(pin)){
                    return BadRequest("Invalid Pin");
                }


                user.Deposit(amount);
                _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(uint amount, int pin)
        {
            string accountNumber = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User user;
            try
            {
                user = await _userRepository.GetUserByAccountNumberAsync(long.Parse(accountNumber));

                if (user == null)
                {
                    return NotFound();
                }
                else if (!user.MatchPin(pin))
                {
                    return BadRequest("Invalid Pin");
                }

                user.Withdraw(amount);
                _userRepository.UpdateUserAsync(user);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("transfer")]
        public async Task <IActionResult> Transfer(long receivingAccount, uint amount, int pin)
        {
            string senderAccount = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User? sendingUser;
            User? ReceivingUser;
            try
            {

                sendingUser = await _userRepository.GetUserByAccountNumberAsync(long.Parse(senderAccount));
                if (sendingUser == null)
                {
                    return NotFound();
                }
                else if (!sendingUser.MatchPin(pin))
                {
                    return BadRequest("Invalid Pin");
                }

            }
            catch
            {
                return BadRequest("Invalid sending user");
            }

            try
            {
                ReceivingUser = await _userRepository.GetUserByAccountNumberAsync(receivingAccount);
                if (ReceivingUser == null)
                {
                    return NotFound();
                }
            }
            catch
            {
                return BadRequest("Invalid Receiving user");
            }


            try
            {
                _transactionManager.Transfer(sendingUser, ReceivingUser, amount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("changePin")]
        public async Task<ActionResult<User>> ChangePin(int pin)
        {
            //// Extract the account number from the token.
            var uAccountNumber = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            User? user;

            try
            {
                 user = await _userRepository.GetUserByAccountNumberAsync(long.Parse(uAccountNumber));

                if(user == null)
                {
                    return NotFound();
                }

                if (user.MatchPin(pin))
                {
                    return BadRequest("Pin cannot be the same as old pin");
                }

               _transactionManager.ChangePin(user, pin);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

            return Ok(user);
        }



    }
}
