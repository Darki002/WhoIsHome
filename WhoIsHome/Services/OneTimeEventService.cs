using WhoIsHome.Aggregates;

namespace WhoIsHome.Services;

public class OneTimeEventService : IService<OneTimeEvent>
{
    public Task<OneTimeEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<OneTimeEvent> CreateAsync(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        DinnerTime dinnerTime, int userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<OneTimeEvent> UpdateAsync(int id, string title, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, DinnerTime dinnerTime, int userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}