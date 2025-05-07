using WhoIsHome.Host.BackgroundTasks.DatabaseCleanUpTask;
using WhoIsHome.Host.BackgroundTasks.QueuedTasks;
using WhoIsHome.Shared.BackgroundTasks;

namespace WhoIsHome.Host.BackgroundTasks;

public static class BackgroundTaskServiceProviderConfig
{
    public static IServiceCollection AddBackgroundTasks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<QueuedHostedService>();
        
        services.Configure<DbCleanUpTaskOptions>(configuration.GetSection("DbCleanUp"));
        services.AddHostedService<DbCleanUpTask>();
        
        return services;
    }
}