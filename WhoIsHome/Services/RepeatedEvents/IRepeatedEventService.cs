using Galaxus.Functional;

namespace WhoIsHome.Services.RepeatedEvents;

public interface IRepeatedEventService : IService<RepeatedEvent>
{
    Task<Result<RepeatedEvent, string>> CreateAsync(
        string eventName,
        string personId,
        DateTime startDate,
        DateTime endDate,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime dinnerAt,
        CancellationToken cancellationToken);

    Task<Result<RepeatedEvent, string>> UpdateAsync(
        string id,
        string eventName,
        DateTime startDate,
        DateTime endDate,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime dinnerAt,
        CancellationToken cancellationToken);
}