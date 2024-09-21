using WhoIsHome.Aggregates;

namespace WhoIsHome.Services;

public class RepeatedEventService : IService<RepeatedEvent>
{
    public Task<RepeatedEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<RepeatedEvent> CreateAsync(string title, DateOnly firstOccurrence, DateOnly lastOccurrence,
        TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime, int userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<RepeatedEvent> UpdateAsync(int id, string title, DateOnly firstOccurrence,
        DateOnly lastOccurrence, TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime, int userId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}