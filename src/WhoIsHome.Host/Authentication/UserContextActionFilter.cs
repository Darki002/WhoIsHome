using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WhoIsHome.Host.Authentication;

public class UserContextActionFilter(UserContext userContext, ILogger<UserContextActionFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                .Any(a => a.GetType() == typeof(AuthorizeAttribute));

            if (!isDefined)
            {
                await next();
                return;
            }
        }
        
        var idString = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(idString) || !int.TryParse(idString, out var id))
        {
            logger.LogInformation("(Invalid Claims) | No User ID found in request");

            var error = new
            {
                error = "Unauthorized",
                message = "User ID was not found in the request claims."
            };

            context.Result = new UnauthorizedObjectResult(error);
            return;
        }
        
        userContext.UserId = id;
        await next();
    }
}