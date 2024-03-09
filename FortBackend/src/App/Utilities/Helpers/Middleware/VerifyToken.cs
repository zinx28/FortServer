using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Utilities.Helpers.Middleware
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class VerifyTokenAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           // if (context.HttpContext.Request.Headers["Au"])
            //if (0)
            //{
                await next();
            //}
            //else
            //{
            //    context.Result = new UnauthorizedResult();
            //}
        }
    }
}
