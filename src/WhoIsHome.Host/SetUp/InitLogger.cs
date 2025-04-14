using Microsoft.Extensions.Logging.Console;

namespace WhoIsHome.Host.SetUp;

public static class InitLogger
{
    public static WebApplicationBuilder AddLoggers(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "dd.MM.yyyy HH:mm:ss";
            options.ColorBehavior = LoggerColorBehavior.Enabled;
        });
        
        if (!builder.Environment.IsDevelopment())
        {
            builder.WebHost.UseSentry();
            builder.Logging.AddSentry();
        }

        return builder;
    }
}