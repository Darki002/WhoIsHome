using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.Host.Authentication;

public class ApiKeyMiddleware(IConfiguration configuration, ILogger<ApiKeyMiddleware> logger) : IAsyncAuthorizationFilter
{
    public const string ApiKeyHeaderName = "X-API-KEY";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Request.Path.StartsWithSegments("/health"))
        {
            return;
        }
        
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            logger.LogWarning("No API Key  in request header!");
            
            var error = new
            {
                error = "Unauthorized",
                message = "API Key is missing."
            };

            context.Result = new UnauthorizedObjectResult(error);
            return;
        }

        var apiKey = configuration.GetApiKey();
        if (!apiKey.Equals(extractedApiKey))
        {
            logger.LogInformation("Unauthorized access with wrong API Key");
            
            var error = new
            {
                error = "Unauthorized",
                message = "Unauthorized access (invalid API Key)."
            };

            context.Result = new UnauthorizedObjectResult(error);
        }
    }
}