using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.Handlers;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Services;

internal class RepeatedEventAggregateService(
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    EventUpdateHandler eventUpdateHandler, 
    IUserContext userContext) 
    : IRepeatedEventAggregateService
{
    public async Task<RepeatedEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.RepeatedEvents
            .Include(e => e.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new NotFoundException($"No RepeatedEvent found with the id {id}.");

        return result.ToAggregate();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.RepeatedEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new NotFoundException($"No RepeatedEvent found with the id {id}.");

        if (!userContext.IsUserPermitted(result.UserId))
        {
            throw new ActionNotAllowedException($"User with ID {result.UserId} is not allowed to delete or modify the content of {id}");
        }
        
        context.RepeatedEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
        
        await eventUpdateHandler.HandleAsync(result.ToAggregate(), EventUpdateHandler.UpdateAction.Delete);
    }

    public async Task<RepeatedEvent> CreateAsync(string title, DateOnly firstOccurrence, DateOnly lastOccurrence,
        TimeOnly startTime, TimeOnly endTime, PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users.SingleAsync(u => u.Id == userContext.UserId, cancellationToken: cancellationToken);
        var repeatedEvent = RepeatedEvent
            .Create(title, firstOccurrence, lastOccurrence, startTime, endTime, presenceType, time, user.Id)
            .ToModel();

        var result = await context.RepeatedEvents.AddAsync(repeatedEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        var createdEvent = result.Entity.ToAggregate();

        await eventUpdateHandler.HandleAsync(createdEvent, EventUpdateHandler.UpdateAction.Create);
        
        return createdEvent;
    }

    public async Task<RepeatedEvent> UpdateAsync(int id, string title, DateOnly firstOccurrence,
        DateOnly lastOccurrence, TimeOnly startTime, TimeOnly endTime, PresenceType presenceType, TimeOnly? time,
        CancellationToken cancellationToken)
    {
        var aggregate = await GetAsync(id, cancellationToken);
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users.SingleAsync(u => u.Id == aggregate.UserId, cancellationToken: cancellationToken);
        
        if (!userContext.IsUserPermitted(user.Id))
        {
            throw new ActionNotAllowedException($"User with ID {user.Id} is not allowed to delete or modify the content of {id}");
        }

        aggregate.Update(title, firstOccurrence, lastOccurrence, startTime, endTime, presenceType, time);
        
        var result = context.RepeatedEvents.Update(aggregate.ToModel());
        await context.SaveChangesAsync(cancellationToken);
                
        var updatedEvent = result.Entity.ToAggregate();
        
        await eventUpdateHandler.HandleAsync(updatedEvent, EventUpdateHandler.UpdateAction.Update);

        return updatedEvent;
    }
}