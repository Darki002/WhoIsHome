using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.External.Database;
using WhoIsHome.Host.DataProtectionKeys;

namespace WhoIsHome.Host.SetUp;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<WhoIsHomeContext>();
        var dataProtectionContext = serviceScope.ServiceProvider.GetRequiredService<DataProtectionKeyContext>();
        
        context.Database.Migrate();
        dataProtectionContext.Database.Migrate();
    }
}