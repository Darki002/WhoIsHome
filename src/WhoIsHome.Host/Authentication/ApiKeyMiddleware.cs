﻿using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.Host.Authentication;

public class ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
{
    public const string ApiKeyHeaderName = "X-API-KEY";

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            logger.LogWarning("No API Key  in request header!");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }


        var apiKey = configuration.GetApiKey();
        if (!apiKey.Equals(extractedApiKey))
        {
            logger.LogInformation("Unauthorized access with wrong API Key");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized access (invalid API Key)");
            return;
        }

        await next(context);
    }
}