using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.PersonOverview;
using WhoIsHome.Services;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services)
    {
        services.AddScoped<OneTimeEventAggregateService>();
        services.AddScoped<RepeatedEventAggregateService>();
        services.AddScoped<UserAggregateService>();

        services.AddTransient<DailyOverviewQueryHandler>();
        services.AddTransient<PersonOverviewQueryHandler>();
        
        return services;
    } 
}