using WhoIsHome.Shared.BackgroundTasks;

namespace WhoIsHome.Host.BackgroundTasks;

public class QueuedHostedService(
    IBackgroundTaskQueue taskQueue,
    ILogger<QueuedHostedService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Queued Hosted Service is running.");
        await BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await taskQueue.DequeueAsync(stoppingToken);

            try
            {
                logger.LogInformation("Start background task {WorkItem}.", nameof(workItem));
                await workItem(stoppingToken);
                logger.LogInformation("Background task {WorkItem} finished successfully.", nameof(workItem));
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error occurred executing {WorkItem}.", nameof(workItem));
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Queued Hosted Service is stopping.");
        
        await base.StopAsync(stoppingToken);
    }
}