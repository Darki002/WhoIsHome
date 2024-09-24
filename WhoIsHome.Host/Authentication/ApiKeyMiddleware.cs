namespace WhoIsHome.Host.Authentication;

public class ApiKeyMiddleware(RequestDelegate next)
{
    public const string ApiKeyHeaderName = "X-API-KEY";
    private readonly string apiKey = Environment.GetEnvironmentVariable("API_KEY") ??
                                     throw new Exception("API_KEY not found in environment variables.");

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized access.");
            return;
        }

        await next(context);
    }
}