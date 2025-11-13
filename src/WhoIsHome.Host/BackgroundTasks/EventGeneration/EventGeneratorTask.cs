using Microsoft.EntityFrameworkCore;
using WhoIsHome.External.Database;
using WhoIsHome.Services;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Host.BackgroundTasks.EventGeneration;

public class EventGeneratorTask(
    IServiceScopeFactory scopeFactory, 
    ILogger<EventGeneratorTask> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now   = DateTime.Now;
            var nextRun = BackgroundTaskHelpers.CalculateNextRun(now, DayOfWeek.Sunday, TimeSpan.FromHours(4));
            var delay   = nextRun - now;

            logger.LogInformation("Next scheduled run: {NextRun} (in {Delay}).", nextRun, delay);

            await Task.Delay(delay, stoppingToken);

            try
            {
                logger.LogInformation("[{Time}] Running event generator…", DateTime.Now);
                await GenerateEvents(stoppingToken);
                logger.LogInformation("Event generator finished.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Event generator failed; will retry next week."); 
            }
        }
    }

    private async Task GenerateEvents(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WhoIsHomeContext>();
        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var eventGroups = await db.EventGroups
            .Where(e => e.EndDate > dateTimeProvider.CurrentDate)
            .ToListAsync(cancellationToken);
        
        logger.LogInformation("Found {GroupCount} Groups to generate events for.", eventGroups.Count);
        
        foreach (var eventGroup in eventGroups)
        {
            await eventService.GenerateNextAsync(eventGroup, cancellationToken);
            logger.LogInformation("Generated next events for Group {GroupId} ({GroupTitle})", eventGroup.Id, eventGroup.Title);
        }
    }
}