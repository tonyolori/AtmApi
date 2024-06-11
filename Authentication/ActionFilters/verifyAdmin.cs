using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Authentication.Enum;

namespace AtmApi.ActionFilters
{
    public class VerifyAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Extract claims
            var claimsIdentity = user.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if the user has the "Admin" role
            var isAdmin = claimsIdentity.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == UserRole.Admin.ToString());

            if (!isAdmin)
            {
                context.Result = new ForbidResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
