using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Host.BackgroundTasks.DatabaseCleanUpTask;

public class DbCleanUpTask(DbCleanUpTaskOptions options, IServiceScopeFactory scopeFactory, ILogger<DbCleanUpTask> logger) : BackgroundService
{
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
        var db = scope.ServiceProvider.GetRequiredService<WhoIsHomeContext>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var cutoff = dateTimeProvider.CurrentDate.AddMonths(-1);
        
        await db.OneTimeEvents
            .Where(e => e.Date < cutoff)
            .ExecuteDeleteAsync(ct);
        
        await db.RepeatedEvents
            .Where(e => e.LastOccurrence < cutoff)
            .ExecuteDeleteAsync(ct);
    }
}