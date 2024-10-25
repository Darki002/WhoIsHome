using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Services;

public interface IOneTimeEventAggregateService : IAggregateService<OneTimeEvent>
{
    Task<OneTimeEvent> CreateAsync(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken);

    Task<OneTimeEvent> UpdateAsync(int id, string title, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken);
}