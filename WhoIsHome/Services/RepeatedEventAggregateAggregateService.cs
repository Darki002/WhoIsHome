using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;

namespace WhoIsHome.Services;

public class RepeatedEventAggregateAggregateService(WhoIsHomeContext context) : IAggregateService<RepeatedEvent>
{
    public async Task<RepeatedEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        // TODO: Check User permission

        var result = await context.RepeatedEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new ArgumentException($"No RepeatedEvent found with the id {id}.", nameof(id));

        return result.ToAggregate<RepeatedEvent>();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        // TODO: Check User permission

        var result = await context.RepeatedEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new ArgumentException($"No RepeatedEvent found with the id {id}.", nameof(id));

        context.RepeatedEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RepeatedEvent> CreateAsync(string title, DateOnly firstOccurrence, DateOnly lastOccurrence,
        TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime, int userId, CancellationToken cancellationToken)
    {
        // TODO: Check if User Exists

        var repeatedEvent = RepeatedEvent
            .Create(title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, userId)
            .ToDbModel<RepeatedEventModel>();

        var result = await context.RepeatedEvents.AddAsync(repeatedEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<RepeatedEvent>();
    }

    public async Task<RepeatedEvent> UpdateAsync(int id, string title, DateOnly firstOccurrence,
        DateOnly lastOccurrence, TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime,
        CancellationToken cancellationToken)
    {
        // TODO: Check User permission
        
        var existingRepeatedEvent = await context.RepeatedEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existingRepeatedEvent is null)
        {
            throw new ArgumentException($"No RepeatedEvent found with the id {id}.", nameof(id));
        }

        var aggregate = existingRepeatedEvent.ToAggregate<RepeatedEvent>();
        aggregate.Update(title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime);
        
        var result = context.RepeatedEvents.Update(aggregate.ToDbModel<RepeatedEventModel>());
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<RepeatedEvent>();
    }
}