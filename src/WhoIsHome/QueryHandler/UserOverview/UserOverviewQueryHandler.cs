using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverviewQueryHandler(IDbContextFactory<WhoIsHomeContext> contextFactory, IDateTimeProvider dateTimeProvider)
{
    public async Task<UserOverviewMock> HandleAsync(int userId, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var today = dateTimeProvider.CurrentDate;

        var eventList = await context.EventInstances
                .Where(e => e.Date >= today)
                .Where(e => e.User.Id == userId)
                .ToListAsync(cancellationToken);

        var todaysEvents = eventList.Where(e => e.Date == dateTimeProvider.CurrentDate)
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

        return new UserOverviewMock
        {
            User = user,
            Today = todaysEvents,
            ThisWeek = thisWeeksEvents,
            FutureEvents = eventsAfterThisWeek
        };
    }

    private static UserOverviewEvent ToUserOverview(EventInstance e) =>
        new()
        {
            Id = e.Id,
            Title = e.Title,
            NextDate = e.Date,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            TemplateId = e.EventGroupId
        };
}