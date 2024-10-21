using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WhoIsHome.AuthTokens;
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

        services.AddTransient<JwtTokenService>();
        services.AddScoped<RefreshTokenService>();
        
        return services;
    } 
}