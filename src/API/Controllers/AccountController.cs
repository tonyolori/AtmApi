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

    }


}
