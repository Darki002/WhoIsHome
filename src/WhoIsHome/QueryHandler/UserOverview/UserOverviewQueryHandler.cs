using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverviewQueryHandler(WhoIsHomeContext context, IDateTimeProvider dateTimeProvider)
{
    public async Task<UserOverview> HandleAsync(int userId, CancellationToken cancellationToken)
    {
        var today = dateTimeProvider.CurrentDate;

        var eventList = await context.EventInstances
            .Include(e => e.EventGroup)
            .Where(e => e.Date >= today)
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);

        var todaysEvents = eventList
            .Where(e => e.Date == dateTimeProvider.CurrentDate)
            .Select(ToUserOverview)
            .ToList();

        var futureEvents = eventList
            .Where(e => e.Date > dateTimeProvider.CurrentDate)
            .ToList();

        var thisWeeksEvents = futureEvents
            .Where(e => e.Date.IsSameWeek(dateTimeProvider.Now))
            .Select(ToUserOverview)
            .ToList();

        var eventsAfterThisWeek = futureEvents
            .Where(e => !e.Date.IsSameWeek(dateTimeProvider.Now))
            .Select(ToUserOverview)
            .ToList();

        var user = await context.Users
            .SingleAsync(u => u.Id == userId, cancellationToken);

        return new UserOverview
        {
            User = user,
            Today = todaysEvents,
            ThisWeek = thisWeeksEvents,
            FutureEvents = eventsAfterThisWeek
        };
    }

    private static UserOverviewEvent ToUserOverview(EventInstance e) =>
        new UserOverviewEvent
        {
            Id = e.Id,
            Title = e.Title,
            NextDate = e.Date,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            TemplateId = e.EventGroupId,
            HasRepetitions = e.EventGroup.HasRepetitions
        };
}