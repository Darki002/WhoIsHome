using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.Persons;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services)
    {
        services.AddSingleton<IPersonService, PersonService>();
        return services;
    } 
}