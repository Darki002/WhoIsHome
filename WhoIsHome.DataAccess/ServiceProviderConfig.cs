using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WhoIsHome.DataAccess;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration builderConfiguration)
    {
        services.AddDbContext<WhoIsHomeContext>();
        return services;
    } 
}