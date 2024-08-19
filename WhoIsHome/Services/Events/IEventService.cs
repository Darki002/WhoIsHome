using Galaxus.Functional;

namespace WhoIsHome.Services.Events;

public interface IEventService : IService<Event>
{
    Task<Result<Event, string>> CreateAsync(
        string eventName,
        string personId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt,
        CancellationToken cancellationToken);

    Task<Result<Event, string>> UpdateAsync(
        string id,
        string eventName,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<Event>, string>> GetByPersonIdAsync(string personId, CancellationToken cancellationToken);
}