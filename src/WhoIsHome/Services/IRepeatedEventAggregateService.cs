using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Services;

public interface IRepeatedEventAggregateService : IAggregateService<RepeatedEvent>
{
    Task<RepeatedEvent> CreateAsync(string title, DateOnly firstOccurrence, DateOnly lastOccurrence,
        TimeOnly startTime, TimeOnly endTime, PresenceType presenceType, TimeOnly? time,
        CancellationToken cancellationToken);

    Task<RepeatedEvent> UpdateAsync(int id, string title, DateOnly firstOccurrence,
        DateOnly lastOccurrence, TimeOnly startTime, TimeOnly endTime, PresenceType presenceType, TimeOnly? time,
        CancellationToken cancellationToken);
}