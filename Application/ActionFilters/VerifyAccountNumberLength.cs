using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AtmApi.ActionFilters
{
    public class VerifyAccountNumberLengthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Get account number from request
            var accountNumber = context.HttpContext.Request.Query["accountNumber"].ToString(); // Change as per your request parameter

            // Check if account number length is not 10
            if (accountNumber.Length != 10)
            {
                context.Result = new BadRequestObjectResult("Account number must be 10 characters long.");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
