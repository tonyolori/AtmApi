﻿using AtmApi.ActionFilters;
using Authentication.ActionFilters;
using Authentication.Enum;
using Authentication.Helpers;
using Authentication.Interfaces;
using Authentication.Logic;
using Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthHelper _authHelper;
        private readonly UniqueAccountGenerator _uniqueAccountGenerator;


        public UserController(IUserRepository userRepository, AuthHelper authHelper, UniqueAccountGenerator uniqueAccountGenerator)
        {
            _userRepository = userRepository;
            _authHelper = authHelper;
            _uniqueAccountGenerator = uniqueAccountGenerator;
        }

        [HttpPost("register")]
        [ValidateEmail]
        [ValidatePassword]// mediator pattern and fluent validation
        public async Task<ActionResult> Register(UserDto user)
        {
            if (user.FirstName.ToLower().Equals(user.LastName.ToLower()))
            {
                return BadRequest("first and last name cannot be the same");
            }
            else if (user.Pin.ToString().Length != 4)
            {
                return BadRequest("invalid pin length");
            }

            var newUser = new User()
            {

                AccountNumber = await _uniqueAccountGenerator.GenerateUniqueAccount(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                Pin = user.Pin,
                Balance = 0,
                Role = UserRole.User,
            };

            User? dbUser = await _userRepository.GetUserByEmailAsync(user.Email);

            if (dbUser != null)
            {
                return BadRequest("User already Exists");
            }

            _userRepository.AddUser(newUser);


            return Ok(newUser);
        }


        [HttpPost("login")]
        [ValidateEmail]
        [ValidatePassword]
        public async Task<ActionResult> Login(string Email, string Password)
        {
            //verify user exists
            bool validateInfo = await _authHelper.ValidateCredentials(Email, Password);

            if (!validateInfo)
            {
                return BadRequest("invalid email or password");
            }

            User? user = await _userRepository.GetUserByEmailAsync(Email);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            var token = _authHelper.GenerateJWTToken(user);

            return Ok(token);
        }

        [HttpPut]
        [ValidateEmail]
        [ValidatePassword]
        public async Task<ActionResult<User>> UpdateUser(UserDto user)
        {
            if (user.FirstName.ToLower().Equals(user.LastName.ToLower()))
            {
                return BadRequest("first and last name cannot be the same");
            }
            else if (user.Pin.ToString().Length != 4)
            {
                return BadRequest("invalid pin length");
            }
            var dbUser = await _userRepository.GetUserByEmailAsync(user.Email);

            if (dbUser == null)
            {
                return NotFound();
            }
            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Email = user.Email;
            dbUser.Password = user.Password;

            _userRepository.UpdateUserAsync(dbUser);

            return Ok(dbUser);
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteUser()
        {
            string accountNumber = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User dbUser;
            try
            {
                dbUser = await _userRepository.GetUserByAccountNumberAsync(long.Parse(accountNumber));
                if (dbUser == null)
                {
                    return NotFound();
                }
            }
            catch
            {
                return NotFound();
            }
            _userRepository.DeleteUser(dbUser);

            return Ok();
        }
    }
}