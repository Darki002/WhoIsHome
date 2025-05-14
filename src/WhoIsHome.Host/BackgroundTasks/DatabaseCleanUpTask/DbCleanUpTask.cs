using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhoIsHome.External;
using WhoIsHome.Host.DataProtectionKeys;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Host.BackgroundTasks.DatabaseCleanUpTask;

public class DbCleanUpTask(IOptions<DbCleanUpTaskOptions> options, IServiceScopeFactory scopeFactory, ILogger<DbCleanUpTask> logger) : BackgroundService
{
    private readonly DbCleanUpTaskOptions options = options.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now     = DateTime.Now;
            var nextRun = CalculateNextRun(now, options.DayOfWeek, options.Time);
            var delay   = nextRun - now;

            logger.LogInformation("Next scheduled run: {NextRun} (in {Delay}).", nextRun, delay);

            await Task.Delay(delay, stoppingToken);

            try
            {
                logger.LogInformation("[{Time}] Running cleanup…", DateTime.Now);
                await RunDatabaseCleanupAsync(stoppingToken);
                logger.LogInformation("Cleanup finished.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cleanup failed; will retry next week.");
            }
        }
    }
    
    private static DateTime CalculateNextRun(DateTime from, DayOfWeek day, TimeSpan at)
    {
        var current = (int)from.DayOfWeek;
        var target  = (int)day;
        var daysUntil = (target - current + 7) % 7;
        if (daysUntil == 0 && from.TimeOfDay >= at) daysUntil = 7;

        return from.Date.AddDays(daysUntil).Add(at);
    }
    
    private async Task RunDatabaseCleanupAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<WhoIsHomeContext>>();
        var db = await dbFactory.CreateDbContextAsync(ct);
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var cutoff = dateTimeProvider.CurrentDate.AddDays(-options.DaysToKeep);
        
        var deletedOneTimeEvents = await db.OneTimeEvents
            .Where(e => e.Date < cutoff)
            .ExecuteDeleteAsync(ct);
        logger.LogInformation("Deleted {Count} one-time events older than {Cutoff}.", deletedOneTimeEvents, cutoff);
        
        var deletedRepeatedEvents = await db.RepeatedEvents
            .Where(e => e.LastOccurrence < cutoff)
            .ExecuteDeleteAsync(ct);
        logger.LogInformation("Deleted {Count} repeated events with last occurrence before {Cutoff}.", deletedRepeatedEvents, cutoff);
    }
}