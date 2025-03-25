using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.External.PushUp;
using WhoIsHome.External.PushUp.ApiClient;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.External;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = BuildConnectionString(configuration);
        services.AddDbContextFactory<WhoIsHomeContext>(o => o.UseMySQL(connectionString));

        services.AddScoped<IPushUpContext, PushUpContext>();
        services.AddSingleton<PushApiClient>();
        
        return services;
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var mysql = configuration.GetMySql();
        return $"Server={mysql.Server};Port={mysql.Port};Database={mysql.Database};User={mysql.User};Password={mysql.Password};";
    }
}