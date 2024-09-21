using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services, IConfiguration builderConfiguration)
    {
        return services;
    } 
}