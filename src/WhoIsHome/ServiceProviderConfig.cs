using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.AuthTokens;
using WhoIsHome.Handlers;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.QueryHandler.WeeklyReports;
using WhoIsHome.Services;
using WhoIsHome.Services.ChoreServices;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services)
    {
        services.AddScoped<IRepeatedEventAggregateService, RepeatedEventAggregateService>();
        services.AddScoped<IUserAggregateService, UserService>();
        services.AddScoped<IChoreService, ChoreService>();

        services.AddTransient<UserDayOverviewQueryHandler>();
        services.AddTransient<DailyOverviewQueryHandler>();
        services.AddTransient<UserOverviewQueryHandler>();
        services.AddTransient<WeeklyReportQueryHandler>();

        services.AddTransient<IEventUpdateHandler, EventUpdateHandler>();

        services.AddTransient<JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        
        return services;
    } 
}