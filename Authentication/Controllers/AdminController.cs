using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Users.Queries;
using Domain.Models;
using Domain.Enum;
using Application.UserTransactions.Commands;
using Application.Common.Models;
using Application.UserTransactions.Queries;
using Application.Users.Commands;

namespace AtmApi.Controllers;

//[VerifyAdmin]
[Route("api/[controller]")]
[ApiController]
public class AdminController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount(UserDto account)
    {
        try
        {
            var command = new AddUserCommand(account, UserRole.Admin);
            Result result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers(int? role,string? searchValue)
    {
        try
        {
            var query = new GetAllUsersQuery { UserRole = (int)role  , SearchValue = searchValue };
            Result result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(long accountNumber)
    {
        try
        {
            var query = new GetBalanceQuery(accountNumber);
            Result result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(long accountNumber, uint amount)
    {
        try
        {
            var depositCommand = new DepositCommand(accountNumber, amount);
            Result result = await _mediator.Send(depositCommand);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(long accountNumber, uint amount)
    {
        try
        {
            var withdrawCommand = new WithdrawCommand(accountNumber, amount);
            Result result = await _mediator.Send(withdrawCommand);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(long sendingAccount, long receivingAccount, uint amount, int pin)
    {
        try
        {
            var TransferCommand = new TransferCommand(sendingAccount, receivingAccount, amount, pin);
            Result result = await _mediator.Send(TransferCommand);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("changePin")]
    public async Task<ActionResult<User>> ChangePin(long accountNumber, int pin)
    {
        try
        {
            var ChangePinCommand = new ChangePinCommand(accountNumber, pin);
            Result result = await _mediator.Send(ChangePinCommand);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUser(long accountNumber)
    {
        try
        {
            var deleteUserCommand = new DeleteUserCommand(accountNumber);
            Result result = await _mediator.Send(deleteUserCommand);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}




//using AtmApi.ActionFilters;
//using Application.Helpers;
//using Domain.Enum;
//using Domain.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Application.Interfaces;
//using FluentValidation.Results;
//using Application.Validator;
//using Application;

//namespace AtmApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]

//    public class AdminController(IUserRepository userRepository,
//        AccountGeneratorHelper uniqueAccountGenerator,
//        TransactionHelper transactionManager, ITransactionRepository transactionRepository) : Controller
//    {
//        private readonly AccountGeneratorHelper _uniqueAccountGenerator = uniqueAccountGenerator;
//        private readonly IUserRepository _userRepository = userRepository;
//        private readonly TransactionHelper _transactionManager = transactionManager;
//        private readonly ITransactionRepository _transactionRepository = transactionRepository;



//        [HttpPost("create")]
//        public async Task<IActionResult> CreateAccount(UserDto account)
//        {


//            UserValidator validator = new();

//            ValidationResult result = validator.Validate(account);

//            if (!result.IsValid)
//            {
//                string errors = "";
//                GetErrorList(result.Errors).ForEach(x => errors += "\n" + x);
//                return BadRequest(errors);
//            }


//            User user = new()
//            {
//                FirstName = account.FirstName,
//                LastName = account.LastName,
//                AccountNumber = await _uniqueAccountGenerator.GenerateUniqueAccount(),
//                Email = account.Email,
//                Balance = 0,
//                Pin = account.Pin,
//                Password = account.Password,
//                Role = UserRole.Admin,
//            };

//            _userRepository.AddUser(user);

//            return Created("", user);
//        }

//        [Authorize]
//        [VerifyAdmin]
//        [HttpGet("GetUsers")]
//        public async Task<ActionResult<List<User>>> GetAllUsers()
//        {
//            var Users = await _userRepository.GetUsersAsync();

//            return Ok(Users);
//        }

//        [Authorize]
//        [VerifyAdmin]
//        [HttpGet("GetAllAdminAccounts")]
//        public async Task<IActionResult> GetAllAdmins()
//        {

//            List<long> admins = await _userRepository.GetAllAdminsAsync();

//            return Ok(admins);

//        }

//        [Authorize]
//        [VerifyAdmin]
//        [HttpGet("GetAllAccountNumbers")]
//        public async Task<ActionResult<List<long>>> GetAccountNumbers()
//        {
//            List<User> users = await _userRepository.GetUsersAsync();
//            List<long> accountNumbers = users.Select(x => x.AccountNumber).ToList();
//            return Ok(accountNumbers);
//        }

//        [Authorize]
//        [VerifyAdmin]
//        [VerifyAccountNumberLength]
//        [HttpGet("GetAccountType")]
//        public async Task<ActionResult<string>> GetAccountType(long accountNumber)
//        {
//            if (!IsValidAccountnumber(accountNumber))
//            {
//                return BadRequest("Invalid Account Number");
//            }

//            var users = await _userRepository.GetUsersAsync();

//            var user = users.FirstOrDefault(u => u.AccountNumber == accountNumber);

//            if (user != null)
//            {
//                var userType = user.Role.ToString();

//                return Ok(userType);
//            }
//            else
//            {
//                // User not found
//                return NotFound("User not found.");
//            }
//        }

//        [HttpGet("balance")]
//        [Authorize]
//        [VerifyAdmin]
//        [VerifyAccountNumberLength]
//        public async Task<IActionResult> GetBalance(long accountNumber)
//        {
//            if (!IsValidAccountnumber(accountNumber))
//            {
//                return BadRequest("Invalid Account Number");
//            }

//            User user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);

//            if(user == null){
//                return NotFound("user not found");
//            }

//            long balance = user.GetBalance();

//            var transaction = TransactionType.BalanceQuery.CreateTransaction(accountNumber,balance,true);
//            await _transactionRepository.AddTransaction(transaction);
//            return Ok(user.GetBalance());
//        }

//        [Authorize]
//        [VerifyAdmin]
//        [VerifyAccountNumberLength]
//        [HttpPost("deposit")]
//        public async Task<IActionResult> Deposit(long accountNumber, uint amount)
//        {
//            if (!IsValidAccountnumber(accountNumber))
//            {
//                return BadRequest("Invalid Account Number");
//            }

//            User user;
//            try
//            {
//                user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);
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

//        [Authorize]
//        [VerifyAdmin]
//        [VerifyAccountNumberLength]
//        [HttpPost("withdraw")]
//        public async Task<IActionResult> Withdraw(long accountNumber, uint amount)
//        {
//            if (!IsValidAccountnumber(accountNumber))
//            {
//                return BadRequest("Invalid Account Number");
//            }

//            User? user = await _userRepository.GetUserByAccountNumberAsync(accountNumber);

//            if(user == null){
//                return NotFound();
//            }

//            try
//            {
//                user.Withdraw(amount);
//                _userRepository.UpdateUserAsync(user);

//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }

//            var transaction = TransactionType.Withdrawal.CreateTransaction(accountNumber, amount, true);
//            await _transactionRepository.AddTransaction(transaction);

//            return Ok();
//        }

//        [Authorize]
//        [VerifyAdmin]
//        [HttpPost("transfer")]
//        public async Task<IActionResult> Transfer(long sendingAccount, long receivingAccount, uint amount)
//        {

//            if (sendingAccount.ToString().Length != 10 || receivingAccount.ToString().Length != 10)
//            {
//                return BadRequest("Invalid Account Number Length");
//            }
//            if(sendingAccount == receivingAccount)
//            {
//                return BadRequest("Cannot Transfer to the same account");
//            }


//            User sendingUser = await _userRepository.GetUserByAccountNumberAsync(sendingAccount);

//            if(sendingUser == null)
//            {
//                return BadRequest("Invalid sending user");
//            }



//            User ReceivingUser = await _userRepository.GetUserByAccountNumberAsync(receivingAccount);
//            if (ReceivingUser == null)
//            {
//                return BadRequest("Invalid Receiving user");
//            }


//            if (_transactionManager.Transfer(sendingUser, ReceivingUser, amount))
//            {
//                var transaction = TransactionType.Deposit.CreateTransaction(sendingAccount, receivingAccount, amount, true);
//                await _transactionRepository.AddTransaction(transaction);
//                return Ok(CustomResponseMessage.Sucess("Transfer"));
//            }
//            else
//            {
//                return BadRequest();
//            }

//        }

//        [HttpPost("changePin")]
//        [VerifyAccountNumberLength]

//        public async Task<ActionResult<User>> ChangePin(long accountNumber, int pin)
//        {
//            if (!IsValidAccountnumber(accountNumber))
//            {
//                return BadRequest("Invalid Account Number");
//            }

//            User account;
//            try
//            {
//                account = await _userRepository.GetUserByAccountNumberAsync(accountNumber);

//                if (account == null)
//                {
//                    return NotFound();
//                }
//                if (account.MatchPin(pin))
//                {
//                    return BadRequest("Pin cannot be the same as old pin");
//                }

//                _transactionManager.ChangePin(account, pin);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }

//            return Ok(account);
//        }

//        [VerifyAccountNumberLength]
//        [HttpDelete]
//        public async Task<ActionResult> DeleteUser(long accountNumber)
//        {
//            if (!IsValidAccountnumber(accountNumber))
//            {
//                return BadRequest("Invalid Account Number");
//            }

//            var dbUser = await _userRepository.GetUserByAccountNumberAsync(accountNumber);

//            if (dbUser == null)
//            {
//                return NotFound();
//            }

//            _userRepository.DeleteUser(dbUser);

//            return Ok();
//        }

//        private static List<string> GetErrorList(List<ValidationFailure> errors)
//        {
//            List<string> errorList = [];
//            foreach (ValidationFailure failure in errors)
//            {
//                //{failure.PropertyName}:
//                errorList.Add($"{failure.ErrorMessage}");
//            }

//            return errorList;
//        }

//        private static bool IsValidAccountnumber(long accountNumber)
//        {
//            return accountNumber.ToString().Length == 10;
//        }

//    }


//}