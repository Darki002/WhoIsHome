namespace WhoIsHome.Host.Authentication;

public class ApiKeyMiddleware(RequestDelegate next)
{
    private const string ApiKeyHeaderName = "X-API-KEY";
    
    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }

        var apiKey = Environment.GetEnvironmentVariable("API_KEY");

        if (apiKey is null || !apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized access.");
            return;
        }

        await next(context);
    }
}