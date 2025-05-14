using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.Host.DataProtectionKeys;

namespace WhoIsHome.Host.SetUp;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<WhoIsHomeContext>();
        context.Database.Migrate();
    }
}