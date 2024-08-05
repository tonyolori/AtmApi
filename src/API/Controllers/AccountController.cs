using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AtmApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [Authorize]
        [HttpPost("getAccountNumber")]
        public ActionResult<long> GetAccountNumber()
        {
            string? accountNumber = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(accountNumber);
        }

        [Authorize]
        [HttpPost("GetAccountType")]
        public ActionResult<List<string>> GetAccountType()
        {
            // Retrieve the role claim
            Claim? roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

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

    }


}
