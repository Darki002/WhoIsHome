using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.DataAccess;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = BuildConnectionString(configuration);
        services.AddDbContext<WhoIsHomeContext>(o => o.UseMySQL(connectionString));
        return services;
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var server = configuration.GetMySqlServer();
        var port = configuration.GetMySqlPort();
        var database = configuration.GetMySqlDatabase();
        var user = configuration.GetMySqlUser();
        var password = configuration.GetMySqlPassword();

        return $"Server={server};Port={port};Database={database};User={user};Password={password};";
    }
}