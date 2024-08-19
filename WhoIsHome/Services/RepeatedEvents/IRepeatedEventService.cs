using Galaxus.Functional;

namespace WhoIsHome.Services.RepeatedEvents;

public interface IRepeatedEventService : IService<RepeatedEvent>
{
    Task<Result<RepeatedEvent, string>> CreateAsync(
        string eventName,
        string personId,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt,
        CancellationToken cancellationToken);

    Task<Result<RepeatedEvent, string>> UpdateAsync(
        string id,
        string eventName,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<RepeatedEvent>, string>> GetByPersonIdAsync(string personId, CancellationToken cancellationToken);
}