namespace WhoIsHome.Host.Authentication;

public class ApiKeyMiddleware
{
    public const string ApiKeyHeaderName = "X-API-KEY";
    private readonly string apiKey;
    private readonly RequestDelegate next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        this.next = next;
        apiKey = Environment.GetEnvironmentVariable("API_KEY") ??
                 throw new Exception("API_KEY not found in environment variables.");
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized access.");
            return;
        }

        await next(context);
    }
}