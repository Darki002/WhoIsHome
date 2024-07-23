using Galaxus.Functional;

namespace WhoIsHome.Services.Events;

public interface IEventService : IService<Event>
{
    Task<Result<Event, string>> CreateAsync(
        string eventName,
        string personId,
        DateTime date,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime? dinnerAt,
        CancellationToken cancellationToken);

    Task<Result<Event, string>> UpdateAsync(
        string id,
        string eventName,
        DateTime date,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime? dinnerAt,
        CancellationToken cancellationToken);
}