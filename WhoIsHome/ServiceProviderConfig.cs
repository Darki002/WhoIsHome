using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WhoIsHome.Services;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services)
    {
        services.AddScoped<OneTimeEventAggregateAggregateService>();
        services.AddScoped<RepeatedEventAggregateAggregateService>();
        services.AddScoped<UserAggregateService>();
        return services;
    } 
}