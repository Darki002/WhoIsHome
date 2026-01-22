using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

public interface IEventService
{
    Task GenerateNewAsync(EventGroup eventGroup);

    Task GenerateUpdateAsync(EventGroup eventGroup);

    Task GenerateNextAsync(EventGroup eventGroup, CancellationToken cancellationToken);

    Task DeleteAsync(int eventGroupId);
    
    Task<IReadOnlyList<EventInstance>?> PredictNextAsync(int eventGroupId, int weeks);
}