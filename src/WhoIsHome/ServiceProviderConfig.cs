using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhoIsHome.AuthTokens;
using WhoIsHome.External.Database;
using WhoIsHome.External.PushUp;
using WhoIsHome.External.PushUp.ApiClient;
using WhoIsHome.External.Translation;
using WhoIsHome.Handlers;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.QueryHandler.WeeklyReports;
using WhoIsHome.Services;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome;

public static class ServiceProviderConfig
{
    public static IServiceCollection AddWhoIsHomeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventGroupService, EventGroupService>();
        services.AddScoped<IUserService, UserService>();

        services.AddTransient<UserDayOverviewQueryHandler>();
        services.AddTransient<DailyOverviewQueryHandler>();
        services.AddTransient<UserOverviewQueryHandler>();
        services.AddTransient<WeeklyReportQueryHandler>();

        services.AddTransient<IEventUpdateHandler, EventUpdateHandler>();

        services.AddTransient<JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        
        return services.AddInfraServices(configuration);
    } 
    
    private static IServiceCollection AddInfraServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = BuildConnectionString(configuration);
        services.AddDbContext<WhoIsHomeContext>(o => o.UseMySQL(connectionString));

        services.AddSingleton<ITranslationService, TranslationService>();
        
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