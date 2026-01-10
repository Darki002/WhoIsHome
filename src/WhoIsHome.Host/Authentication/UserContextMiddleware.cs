using System.Security.Claims;

namespace WhoIsHome.Host.Authentication;

internal class UserContextMiddleware(UserContextInfo userContextInfo) : IMiddleware
{ 
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var idString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (int.TryParse(idString, out var id))
        {
            userContextInfo.Init(id);
        }
        
        await next(context);
    }
}