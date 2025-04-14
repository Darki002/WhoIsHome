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
                logger.LogWarning(ex, "Background task {WorkItem} failed on first attempt. Retrying once in 10sec.", nameof(workItem));
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                try
                {
                    logger.LogInformation("Start background task {WorkItem} retry.", nameof(workItem));
                    await workItem(stoppingToken);
                    logger.LogInformation("Background task {WorkItem} succeeded on retry.", nameof(workItem));
                }
                catch (Exception retryEx)
                {
                    logger.LogError(retryEx, "Background task {WorkItem} failed on retry. Task will not be retried again.", nameof(workItem));
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Queued Hosted Service is stopping.");
        
        await base.StopAsync(stoppingToken);
    }
}
