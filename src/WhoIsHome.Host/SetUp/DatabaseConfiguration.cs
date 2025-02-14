using Microsoft.EntityFrameworkCore;
using WhoIsHome.DataAccess;

namespace WhoIsHome.Host.SetUp;

public static class DatabaseConfiguration
{
    public static async Task ConfigureDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<WhoIsHomeContext>();
        await context.Database.MigrateAsync();
    }
}