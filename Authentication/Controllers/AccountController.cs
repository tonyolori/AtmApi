using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Application.Helpers;
using Application.Interfaces;

namespace AtmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthHelper _authHelper;
        private readonly AccountGeneratorHelper _uniqueAccountGenerator;
        private readonly IUserRepository _userRepository;


        public AccountController(AuthHelper authHelper, AccountGeneratorHelper uniqueAccountGenerator, IUserRepository userRepository)
        {
            _authHelper = authHelper;
            _uniqueAccountGenerator = uniqueAccountGenerator;
            _userRepository = userRepository;
        }



    [Authorize]
    [HttpGet("getAccountNumber")]
    public ActionResult<long> GetAccountNumber()
    {
        string accountNumber = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok(accountNumber);
    }

    [Authorize]
    [HttpGet("GetAccountType")]
    public async Task<ActionResult<string>> GetAccountType()
    {
        // Retrieve the role claim
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        // Check if the role claim exists
        if (roleClaim != null)
        {
            // Return the user type
            return Ok(roleClaim.Value);
        }
        else
        {
            // Role claim not found
            return BadRequest("User type not found.");
        }
    }

    
    //[HttpGet("getuserType")]
    //[Authorize(Roles = "USER")]
    ////[Authorize(Policy = IdentityData.AdminuserPolicyName)]
    //public async Task<ActionResult<UserAccount>> GetUserType()
    //{
    //    // Retrieve userId from the claims
    //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    //    Console.WriteLine("Claims received:");
    //    foreach (var claim in User.Claims)
    //    {
    //        Console.WriteLine($"{claim.Type}: {claim.Value}");
    //    }

    //    if (userIdClaim == null)
    //    {
    //        return Unauthorized("No user ID claim present in token.");
    //    }

    //    try
    //    {
    //        UserAccount user = AccountManager.GetAccount(long.Parse(userIdClaim));
    //        return Ok(user);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

}
}
