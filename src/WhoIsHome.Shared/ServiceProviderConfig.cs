using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Shared;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeShared(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return services;
    } 
}