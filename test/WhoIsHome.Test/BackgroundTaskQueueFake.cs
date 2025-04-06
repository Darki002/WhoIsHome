using WhoIsHome.Shared.BackgroundTasks;

namespace WhoIsHome.Test;

public class BackgroundTaskQueueFake : IBackgroundTaskQueue
{
    public IReadOnlyCollection<Func<CancellationToken, ValueTask>> Queue => queue;
    
    private List<Func<CancellationToken, ValueTask>> queue = [];
    
    public ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        queue.Add(workItem);
        return ValueTask.CompletedTask;
    }

    public ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(queue.First());
    }
}