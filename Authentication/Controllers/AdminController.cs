using AtmApi.ActionFilters;
using Application.Helpers;
using Domain.Enum;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;

namespace AtmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AdminController(IUserRepository userRepository,
        AccountGeneratorHelper uniqueAccountGenerator,
        TransactionHelper transactionManager) : Controller
    {
        private readonly AccountGeneratorHelper _uniqueAccountGenerator = uniqueAccountGenerator;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly TransactionHelper _transactionManager = transactionManager;


        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount(UserDto account)
        {
            if(account.FirstName.ToLower().Equals(account.LastName.ToLower()))
            {
                return BadRequest("first and last name cannot be the same");
            }
            
            if (account.Pin.ToString().Length != 4)
            {
                return BadRequest("invalid pin length");
            }

            User user = new User
            {
                FirstName = account.FirstName,
                LastName = account.LastName,
                AccountNumber = await _uniqueAccountGenerator.GenerateUniqueAccount(),
                Email = account.Email,
                Balance = 0,
                Pin = account.Pin,
                Password = account.Password,
                Role = UserRole.Admin,
            };


            _userRepository.AddUser(user);

            return Created("", user);
        }

        [Authorize]
        [VerifyAdmin]
        [HttpGet("GetUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var Users = await _userRepository.GetUsersAsync();

            return Ok(Users);
        }

        [Authorize]
        [VerifyAdmin]
        [HttpGet("GetAllAdminAccounts")]
        public async Task<IActionResult> GetAllAdmins()
        {

            List<long> admins = await _userRepository.GetAllAdminsAsync();

            return Ok(admins);

        }

        [Authorize]
        [VerifyAdmin]
        [HttpGet("GetAllAccountNumbers")]
        public async Task<ActionResult<List<long>>> GetAccountNumbers()
        {
            List<User> users = await _userRepository.GetUsersAsync();
            List<long> accountNumbers = users.Select(x => x.AccountNumber).ToList();
            return Ok(accountNumbers);
        }

        [Authorize]
        [VerifyAdmin]
        [VerifyAccountNumberLength]
        [HttpGet("GetAccountType")]
        public async Task<ActionResult<string>> GetAccountType(long accountNumber)
        {
            var users = await _userRepository.GetUsersAsync();

            // Get the user account associated with the provided account number
            var user = users.FirstOrDefault(u => u.AccountNumber == accountNumber);

            // Check if the user account exists
            if (user != null)
            {
                var userType = user.Role.ToString();

                return Ok(userType);
            }
            else
            {
                // User not found
                return NotFound("User not found.");
            }
        }

        ///i stopped here 
        [HttpGet("balance")]
        [Authorize]
        [VerifyAdmin]
        [VerifyAccountNumberLength]
        public async Task<IActionResult> GetBalance(long accountNumber)
        {

            User user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);
                 
            if(user == null){
                return NotFound("user not found");
            }

            return base.Ok(user.GetBalance());
        }

        [Authorize]
        [VerifyAdmin]
        [VerifyAccountNumberLength]
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(long accountNumber, uint amount)
        {
            User user;
            try
            {
                user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);
                user.Deposit(amount);
                _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [VerifyAdmin]
        [VerifyAccountNumberLength]
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(long accountNumber, uint amount)
        {
            User? user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);
                
            if(user == null){
                return NotFound();
            }

            try
            {
                user.Withdraw(amount);
                _userRepository.UpdateUserAsync(user);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [Authorize]
        [VerifyAdmin]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(long sendingAccount, long receivingAccount, uint amount)
        {
            User sendingUser;
            User ReceivingUser;
            
            if (sendingAccount.ToString().Length != 10 || receivingAccount.ToString().Length != 10)
            {
                return BadRequest("Invalid Account Number Length");
            }
            sendingUser = await _userRepository.GetUserByAccountNumberAsync(sendingAccount);

            if(sendingUser == null)
            {
                return BadRequest("Invalid sending user");
            }



            ReceivingUser = await _userRepository.GetUserByAccountNumberAsync(receivingAccount);
            if (ReceivingUser == null)
            {
                return BadRequest("Invalid Receiving user");
            }


            try
            {
                await transactionManager.Transfer(sendingUser, ReceivingUser, amount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("changePin")]
        [VerifyAccountNumberLength]

        public async Task<ActionResult<User>> ChangePin(long accountNumber, int pin)
        {
            User account;
            try
            {
                account = await _userRepository.GetUserByAccountNumberAsync(accountNumber);

                if (account == null)
                {
                    return NotFound();
                }
                if (account.MatchPin(pin))
                {
                    return BadRequest("Pin cannot be the same as old pin");
                }
                // Proceed with changing the PIN.
                _transactionManager.ChangePin(account, pin);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(account);
        }
        [VerifyAccountNumberLength]
        [HttpDelete]
        public async Task<ActionResult> DeleteUser(long accountNumber)
        {
            var dbUser = await _userRepository.GetUserByAccountNumberAsync(accountNumber);

            if (dbUser == null)
            {
                return NotFound();
            }

            _userRepository.DeleteUser(dbUser);

            return Ok();
        }

    }


}