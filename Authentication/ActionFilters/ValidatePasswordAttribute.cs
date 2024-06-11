using Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Authentication.ActionFilters
{
    public class ValidatePasswordAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("Password"))
            {
                // Direct password string provided
                var password = context.ActionArguments["Password"] as string;
                ValidatePassword(password,context);
            }
            else if (context.ActionArguments.ContainsKey("user"))
            {
                // Check for password within user object
                UserDto user = context.ActionArguments["user"] as UserDto; // Replace with your actual user type
                if (user != null)
                {
                    ValidatePassword(user.Password,context);
                }
                else
                {
                    context.Result = new BadRequestObjectResult("Password not provided.");

                }
            }
            else
            {
                // No password provided (directly or within user object)
                context.Result = new BadRequestObjectResult("Password not provided.");
            }

            base.OnActionExecuting(context);
        }

        private void ValidatePassword(string password, ActionExecutingContext context )
        {
            if (string.IsNullOrEmpty(password))
            {
                context.Result = new BadRequestObjectResult("Password cannot be empty.");
                return;
            }

            if (password.Length < 8)
            {
                context.Result = new BadRequestObjectResult("Password must be at least 8 characters long.");
                return;
            }

            if (!password.Any(char.IsDigit))
            {
                context.Result = new BadRequestObjectResult("Password must contain at least one digit.");
                return;
            }

            if (!password.Any(IsSpecialCharacter))
            {
                context.Result = new BadRequestObjectResult("Password must contain at least one special character.");
                return;
            }
        }

        private bool IsSpecialCharacter(char c)
        {
            return !char.IsLetterOrDigit(c);
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Linq;

//namespace Authentication.ActionFilters
//{
//    public class ValidatePasswordAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            if (!context.ActionArguments.ContainsKey("Password"))
//            {
//                context.Result = new BadRequestObjectResult("Password not provided.");
//                return;
//            }

//            var password = context.ActionArguments["Password"] as string;

//            if (string.IsNullOrEmpty(password))
//            {
//                context.Result = new BadRequestObjectResult("Password cannot be empty.");
//                return;
//            }

//            if (password.Length < 8)
//            {
//                context.Result = new BadRequestObjectResult("Password must be at least 8 characters long.");
//                return;
//            }

//            if (!password.Any(char.IsDigit))
//            {
//                context.Result = new BadRequestObjectResult("Password must contain at least one digit.");
//                return;
//            }

//            if (!password.Any(IsSpecialCharacter))
//            {
//                context.Result = new BadRequestObjectResult("Password must contain at least one special character.");
//                return;
//            }

//            base.OnActionExecuting(context);
//        }

//        private bool IsSpecialCharacter(char c)
//        {
//            return !char.IsLetterOrDigit(c);
//        }
//    }
//}
