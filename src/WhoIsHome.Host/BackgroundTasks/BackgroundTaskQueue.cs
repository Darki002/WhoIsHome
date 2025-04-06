using System.Threading.Channels;
using WhoIsHome.Shared.BackgroundTasks;

namespace WhoIsHome.Host.BackgroundTasks;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private const int Capacity = 4;
    
    private readonly Channel<Func<CancellationToken, ValueTask>> queue;

    public BackgroundTaskQueue()
    {
        var options = new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        await queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await queue.Reader.ReadAsync(cancellationToken);
    }
}