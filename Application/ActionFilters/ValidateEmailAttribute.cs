using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Domain.Models;

namespace Application.ActionFilters
{
    public class ValidateEmailAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("email"))
            {
                // Direct password string provided
                string? email = context.ActionArguments["email"] as string;
                ValidateEmail(email, context);
            }
            else if (context.ActionArguments.ContainsKey("user"))
            {
                // Check for password within user object
                User? user = context.ActionArguments["user"] as User; // Replace with your actual user type
                if (user != null)
                {
                    ValidateEmail(user.Email, context);
                }
            }
            else
            {
                // No email provided (directly or within user object)
                context.Result = new BadRequestObjectResult("Email address not provided.");
            }

        }

        private static void ValidateEmail(string? email, ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(email))
            {
                context.Result = new BadRequestObjectResult("Email address cannot be empty.");
                return;
            }

            if (!IsValidEmail(email))
            {
                context.Result = new BadRequestObjectResult("Invalid email address format.");
                return;
            }
        }



        private static bool IsValidEmail(string email)
        {
            // Regular expression pattern for "_@." format
            string pattern = @"^[\w-]+@[\w-]+\.[\w.]+$";

            //string pattern = @"^\w + ([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            // Check if the email matches the pattern
            return Regex.IsMatch(email, pattern);
        }

    }
}
