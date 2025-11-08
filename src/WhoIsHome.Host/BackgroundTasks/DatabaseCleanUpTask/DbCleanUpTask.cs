using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WhoIsHome.External.Database;
using WhoIsHome.Services;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Host.BackgroundTasks.DatabaseCleanUpTask;

public class DbCleanUpTask(
    IOptions<DbCleanUpTaskOptions> options, 
    IServiceScopeFactory scopeFactory, 
    ILogger<DbCleanUpTask> logger) : BackgroundService
{
    private readonly DbCleanUpTaskOptions options = options.Value;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now   = DateTime.Now;
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
    
    private async Task RunDatabaseCleanupAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WhoIsHomeContext>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var cutoff = dateTimeProvider.CurrentDate.AddDays(-options.DaysToKeep);
        
        var eventGroups = await db.EventGroups
            .Include(e => e.Events)
            .Where(e => e.EndDate < cutoff)
            .ToListAsync(stoppingToken);
        
        foreach (var eventGroup in eventGroups)
        {
            db.EventInstances.RemoveRange(eventGroup.Events);
            db.EventGroups.Remove(eventGroup);
            await db.SaveChangesAsync(stoppingToken);
            
            logger.LogInformation(
                "Deleted Event Groups with EndDate {EndDate} and removed {Count} Event Instances.", 
                eventGroup.EndDate, 
                eventGroup.Events.Count);
        }
        
        logger.LogInformation("Deleted {Count} Event Groups older than {Cutoff}.", eventGroups.Count, cutoff);
    }
}