using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Services;

public class OneTimeEventAggregateAggregateService(WhoIsHomeContext context) : IAggregateService<OneTimeEvent>
{
    public async Task<OneTimeEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        // TODO: Check User permission

        var result = await context.OneTimeEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null)
        {
            throw new ArgumentException($"No OneTimeEvent found with the id {id}.", nameof(id));
        }
        
        return result.ToAggregate<OneTimeEvent>();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        // TODO: Check User permission
        
        var result = await context.OneTimeEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null)
        {
            throw new ArgumentException($"No OneTimeEvent found with the id {id}.", nameof(id));
        }
        
        context.OneTimeEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OneTimeEvent> CreateAsync(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        DinnerTime dinnerTime, int userId, CancellationToken cancellationToken)
    {
        // TODO: Check if User Exists

        var oneTimeEvent = OneTimeEvent.Create(title, date, startTime, endTime, dinnerTime, userId)
            .ToDbModel<OneTimeEventModel>();

        var result = await context.OneTimeEvents.AddAsync(oneTimeEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<OneTimeEvent>();
    }

    public async Task<OneTimeEvent> UpdateAsync(int id, string title, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, DinnerTime dinnerTime, CancellationToken cancellationToken)
    {
        // TODO: Check User permission
        
        var existingOneTimeEvent = await context.OneTimeEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existingOneTimeEvent is null)
        {
            throw new ArgumentException($"No OneTimeEvent found with the id {id}.", nameof(id));
        }

        var aggregate = existingOneTimeEvent.ToAggregate<OneTimeEvent>();
        aggregate.Update(title, date, startTime, endTime, dinnerTime);
        
        var result = context.OneTimeEvents.Update(aggregate.ToDbModel<OneTimeEventModel>());
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<OneTimeEvent>();
    }
}