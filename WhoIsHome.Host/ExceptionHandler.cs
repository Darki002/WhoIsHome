using Microsoft.AspNetCore.Diagnostics;
using WhoIsHome.Shared.Exceptions;

namespace WhoIsHome.Host;

public static class ExceptionHandler
{
    public static void Handle(IApplicationBuilder errorApp)
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error is ActionNotAllowedException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Action not allowed: " + exceptionHandlerPathFeature.Error.Message);
            }

            if (exceptionHandlerPathFeature?.Error is NotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Not Found: " + exceptionHandlerPathFeature.Error.Message);
            }

            if (exceptionHandlerPathFeature?.Error is ArgumentException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Bad Request: " + exceptionHandlerPathFeature.Error.Message);
            }
        });
    }
}