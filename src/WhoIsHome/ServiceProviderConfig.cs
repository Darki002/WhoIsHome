using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.AuthTokens;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.Services;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services)
    {
        services.AddScoped<IOneTimeEventAggregateService, OneTimeEventAggregateService>();
        services.AddScoped<IRepeatedEventAggregateService, RepeatedEventAggregateService>();
        services.AddScoped<IUserAggregateService, UserAggregateService>();

        services.AddTransient<UserDayOverviewQueryHandler>();
        services.AddTransient<DailyOverviewQueryHandler>();
        services.AddTransient<UserOverviewQueryHandler>();

        services.AddTransient<JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        
        return services;
    } 
}