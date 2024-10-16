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
        var connectionString = BuildConnectionString();
        services.AddDbContext<WhoIsHomeContext>(o => o.UseMySQL(connectionString!));
        return services;
    }

    private static string BuildConnectionString()
    {
        var server = EnvironmentHelper.GetVariable(EnvVariables.MySqlServer);
        var port = EnvironmentHelper.GetVariable(EnvVariables.MySqlPort);
        var database = EnvironmentHelper.GetVariable(EnvVariables.MySqlDatabase);
        var user = EnvironmentHelper.GetVariable(EnvVariables.MySqlUser);
        var password = EnvironmentHelper.GetVariable(EnvVariables.MySqlPassword);

        return $"Server={server};Port={port};Database={database};User={user};Password={password};";
    }
}