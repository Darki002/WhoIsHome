using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.Handlers;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Services;

internal class OneTimeEventAggregateService(
    IDbContextFactory<WhoIsHomeContext> contextFactory, 
    EventUpdateHandler eventUpdateHandler,
    IUserContext userContext)
    : IOneTimeEventAggregateService
{
    public async Task<OneTimeEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.OneTimeEvents
            .Include(e => e.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new NotFoundException($"No OneTimeEvent found with the id {id}.");

        return result.ToAggregate();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.OneTimeEvents
            .Include(oneTimeEventModel => oneTimeEventModel.User)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new NotFoundException($"No OneTimeEvent found with the id {id}.");

        if (!userContext.IsUserPermitted(result.User.Id))
        {
            throw new ActionNotAllowedException($"User with ID {result.User.Id} is not allowed to delete or modify the content of {id}");
        }

        context.OneTimeEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);

        await eventUpdateHandler.HandleAsync(result.ToAggregate(), EventUpdateHandler.UpdateAction.Delete);
    }

    public async Task<OneTimeEvent> CreateAsync(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken)
    {
        var oneTimeEvent = OneTimeEvent.Create(title, date, startTime, endTime, presenceType, time, userContext.UserId)
            .ToModel();

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.OneTimeEvents.AddAsync(oneTimeEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        var createdEvent = result.Entity.ToAggregate();

        await eventUpdateHandler.HandleAsync(createdEvent, EventUpdateHandler.UpdateAction.Create);
        
        return createdEvent;
    }

    public async Task<OneTimeEvent> UpdateAsync(int id, string title, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken)
    {
        var aggregate = await GetAsync(id, cancellationToken);
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var user = await context.Users.SingleAsync(u => u.Id == aggregate.UserId, cancellationToken: cancellationToken);

        if (!userContext.IsUserPermitted(user.Id))
        {
            throw new ActionNotAllowedException($"User with ID {user.Id} is not allowed to delete or modify the content of {id}");
        }
        
        aggregate.Update(title, date, startTime, endTime, presenceType, time);
        
        var result = context.OneTimeEvents.Update(aggregate.ToModel());
        await context.SaveChangesAsync(cancellationToken);
        
        var updatedEvent = result.Entity.ToAggregate();
        
        await eventUpdateHandler.HandleAsync(updatedEvent, EventUpdateHandler.UpdateAction.Create);

        return updatedEvent;
    }
}