﻿using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.Services;

public class OneTimeEventAggregateAggregateService(WhoIsHomeContext context, IUserService userService)
    : IAggregateService<OneTimeEvent>
{
    public async Task<OneTimeEvent> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.OneTimeEvents
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new ArgumentException($"No OneTimeEvent found with the id {id}.", nameof(id));

        return result.ToAggregate<OneTimeEvent>();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.OneTimeEvents
            .Include(oneTimeEventModel => oneTimeEventModel.UserModel)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) throw new ArgumentException($"No OneTimeEvent found with the id {id}.", nameof(id));

        if (!userService.IsUserPermitted(result.UserModel.Id))
        {
            throw new UnauthorizedAccessException($"User with ID {result.UserModel.Id} is not allowed to delete or modify the content of {id}");
        }

        context.OneTimeEvents.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OneTimeEvent> CreateAsync(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime,
        DinnerTime dinnerTime, CancellationToken cancellationToken)
    {
        var user = await userService.GetCurrentUserAsync(cancellationToken);
        
        var oneTimeEvent = OneTimeEvent.Create(title, date, startTime, endTime, dinnerTime, user.Id)
            .ToDbModel<OneTimeEventModel>();

        var result = await context.OneTimeEvents.AddAsync(oneTimeEvent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<OneTimeEvent>();
    }

    public async Task<OneTimeEvent> UpdateAsync(int id, string title, DateOnly date, TimeOnly startTime,
        TimeOnly endTime, DinnerTime dinnerTime, CancellationToken cancellationToken)
    {
        var existingOneTimeEvent = await context.OneTimeEvents
            .Include(e => e.UserModel)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existingOneTimeEvent is null)
            throw new ArgumentException($"No OneTimeEvent found with the id {id}.", nameof(id));

        if (!userService.IsUserPermitted(existingOneTimeEvent.UserModel.Id))
        {
            throw new UnauthorizedAccessException($"User with ID {existingOneTimeEvent.UserModel.Id} is not allowed to delete or modify the content of {id}");
        }
        
        var aggregate = existingOneTimeEvent.ToAggregate<OneTimeEvent>();
        aggregate.Update(title, date, startTime, endTime, dinnerTime);

        var result = context.OneTimeEvents.Update(aggregate.ToDbModel<OneTimeEventModel>());
        await context.SaveChangesAsync(cancellationToken);
        return result.Entity.ToAggregate<OneTimeEvent>();
    }
}