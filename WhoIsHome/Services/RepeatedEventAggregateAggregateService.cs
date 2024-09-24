using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.Services;

public class RepeatedEventAggregateAggregateService(WhoIsHomeContext context, IUserService userService) : IAggregateService<RepeatedEvent>
{
    public async Task<RepeatedEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.RepeatedEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new ArgumentException($"No RepeatedEvent found with the id {id}.", nameof(id));

        return result.ToAggregate<RepeatedEvent>();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.RepeatedEvents
            .Include(repeatedEventModel => repeatedEventModel.UserModel)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new ArgumentException($"No RepeatedEvent found with the id {id}.", nameof(id));

        if (!userService.IsUserPermitted(result.UserModel.Id))
        {
            throw new UnauthorizedAccessException($"User with ID {result.UserModel.Id} is not allowed to delete or modify the content of {id}");
        }
        
        context.RepeatedEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RepeatedEvent> CreateAsync(string title, DateOnly firstOccurrence, DateOnly lastOccurrence,
        TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime, CancellationToken cancellationToken)
    {
        var user = await userService.GetCurrentUserAsync(cancellationToken);
        
        var repeatedEvent = RepeatedEvent
            .Create(title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime, user.Id)
            .ToDbModel<RepeatedEventModel>();

        var result = await context.RepeatedEvents.AddAsync(repeatedEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<RepeatedEvent>();
    }

    public async Task<RepeatedEvent> UpdateAsync(int id, string title, DateOnly firstOccurrence,
        DateOnly lastOccurrence, TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime,
        CancellationToken cancellationToken)
    {
        var existingRepeatedEvent = await context.RepeatedEvents
            .Include(repeatedEventModel => repeatedEventModel.UserModel)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existingRepeatedEvent is null)
        {
            throw new ArgumentException($"No RepeatedEvent found with the id {id}.", nameof(id));
        }
        
        if (!userService.IsUserPermitted(existingRepeatedEvent.UserModel.Id))
        {
            throw new UnauthorizedAccessException($"User with ID {existingRepeatedEvent.UserModel.Id} is not allowed to delete or modify the content of {id}");
        }

        var aggregate = existingRepeatedEvent.ToAggregate<RepeatedEvent>();
        aggregate.Update(title, firstOccurrence, lastOccurrence, startTime, endTime, dinnerTime);
        
        var result = context.RepeatedEvents.Update(aggregate.ToDbModel<RepeatedEventModel>());
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<RepeatedEvent>();
    }
}