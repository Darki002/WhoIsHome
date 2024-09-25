using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Host;

public static class ExceptionHandler
{
    public static void UseExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
            
                    if(exception is null) return;
            
                    int statusCode;
                    string message;
            
                    switch (exception)
                    {
                        case ActionNotAllowedException:
                            statusCode = StatusCodes.Status403Forbidden;
                            message = $"Action not allowed: {exception.Message}";
                            break;
                        case InvalidClaimsException:
                            statusCode = StatusCodes.Status403Forbidden;
                            message = $"Invalid Claims: {exception.Message}";
                            break;
                        case NotFoundException:
                            statusCode = StatusCodes.Status404NotFound;
                            message = $"Not Found: {exception.Message}";
                            break;
                        case InvalidModelException:
                            statusCode = StatusCodes.Status400BadRequest;
                            message = $"Bad Request: {exception.Message}";
                            break;
                        default:
                            statusCode = StatusCodes.Status500InternalServerError;
                            message = "An unexpected error occurred.";
                            // TODO Logger
                            break;
                    }
            
                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        statusCode,
                        error = message
                    });
                    await context.Response.WriteAsync(result);
                }));
    }
}