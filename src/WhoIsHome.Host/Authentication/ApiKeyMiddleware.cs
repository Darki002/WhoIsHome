using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.Host.Authentication;

// ReSharper disable once ClassNeverInstantiated.Global
public class ApiKeyMiddleware(IConfiguration configuration, ILogger<ApiKeyMiddleware> logger) : IMiddleware
{
    public const string ApiKeyHeaderName = "X-API-KEY";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            logger.LogWarning("No API Key  in request header!");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("API Key is missing");
            return;
        }

        var apiKey = configuration.GetApiKey();
        if (!apiKey.Equals(extractedApiKey))
        {
            logger.LogInformation("Unauthorized access with wrong API Key");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Unauthorized access (invalid API Key).");
            return;
        }
        
        await next(context);
    }
}