using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.DataAccess;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Services;

public class OneTimeEventAggregateService(WhoIsHomeContext context, IUserContext userContext)
    : IAggregateService<OneTimeEvent>
{
    public async Task<OneTimeEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.OneTimeEvents
            .Include(e => e.UserModel)
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new NotFoundException($"No OneTimeEvent found with the id {id}.");

        return result.ToAggregate();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.OneTimeEvents
            .Include(oneTimeEventModel => oneTimeEventModel.UserModel)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new NotFoundException($"No OneTimeEvent found with the id {id}.");

        if (!userContext.IsUserPermitted(result.UserModel.Id))
        {
            throw new ActionNotAllowedException($"User with ID {result.UserModel.Id} is not allowed to delete or modify the content of {id}");
        }

        context.OneTimeEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OneTimeEvent> CreateAsync(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken)
    {
        var user = await context.Users.SingleAsync(u => u.Id == userContext.UserId, cancellationToken: cancellationToken);
        var oneTimeEvent = OneTimeEvent.Create(title, date, startTime, endTime, presenceType, time, userContext.UserId)
            .ToModel(user);

        var result = await context.OneTimeEvents.AddAsync(oneTimeEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate();
    }

    public async Task<OneTimeEvent> UpdateAsync(int id, string title, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken)
    {
        var aggregate = await GetAsync(id, cancellationToken);
        var user = await context.Users.SingleAsync(u => u.Id == aggregate.UserId, cancellationToken: cancellationToken);

        if (!userContext.IsUserPermitted(user.Id))
        {
            throw new ActionNotAllowedException($"User with ID {user.Id} is not allowed to delete or modify the content of {id}");
        }
        
        aggregate.Update(title, date, startTime, endTime, presenceType, time);
        
        var result = context.OneTimeEvents.Update(aggregate.ToModel(user));
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate();
    }
}