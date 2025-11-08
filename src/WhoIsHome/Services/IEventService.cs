using WhoIsHome.Entities;

namespace WhoIsHome.Services;

public interface IEventService
{
    Task GenerateNewAsync(EventGroup eventGroup, CancellationToken cancellationToken);

    Task GenerateUpdateAsync(EventGroup eventGroup, CancellationToken cancellationToken);

    Task DeleteAsync(int eventGroupId);
}