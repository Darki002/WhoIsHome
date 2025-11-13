using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

public interface IEventService
{
    Task GenerateNewAsync(EventGroup eventGroup, CancellationToken cancellationToken);

    Task GenerateUpdateAsync(EventGroup eventGroup, CancellationToken cancellationToken);

    Task DeleteAsync(int eventGroupId);

    Task<ValidationError?> EditSingleInstanceAsync(
        DateOnly originalDate,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        PresenceType presenceType,
        TimeOnly dinnerTime,
        CancellationToken cancellationToken);

    Task DeleteSingleInstanceAsync(DateOnly date, CancellationToken cancellationToken);
}