using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Services;

public class OneTimeEventAggregateService(WhoIsHomeContext context, IUserService userService)
    : IAggregateService<OneTimeEvent>
{
    public async Task<OneTimeEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.OneTimeEvents
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

        if (!userService.IsUserPermitted(result.UserModel.Id))
        {
            throw new ActionNotAllowedException($"User with ID {result.UserModel.Id} is not allowed to delete or modify the content of {id}");
        }

        context.OneTimeEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OneTimeEvent> CreateAsync(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken)
    {
        var user = await userService.GetCurrentUserAsync(cancellationToken);
        
        var oneTimeEvent = OneTimeEvent.Create(title, date, startTime, endTime, presenceType, time, user.Id)
            .ToModel(user.ToUser().ToModel());

        var result = await context.OneTimeEvents.AddAsync(oneTimeEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate();
    }

    public async Task<OneTimeEvent> UpdateAsync(int id, string title, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, PresenceType presenceType, TimeOnly? time, CancellationToken cancellationToken)
    {
        var existingOneTimeEvent = await context.OneTimeEvents
            .Include(e => e.UserModel)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existingOneTimeEvent is null)
            throw new NotFoundException($"No OneTimeEvent found with the id {id}.");

        if (!userService.IsUserPermitted(existingOneTimeEvent.UserModel.Id))
        {
            throw new ActionNotAllowedException($"User with ID {existingOneTimeEvent.UserModel.Id} is not allowed to delete or modify the content of {id}");
        }
        
        var aggregate = existingOneTimeEvent.ToAggregate();
        aggregate.Update(title, date, startTime, endTime, presenceType, time);
        
        var user = await userService.GetCurrentUserAsync(cancellationToken);
        var result = context.OneTimeEvents.Update(aggregate.ToModel(user.ToUser().ToModel()));
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate();
    }
}