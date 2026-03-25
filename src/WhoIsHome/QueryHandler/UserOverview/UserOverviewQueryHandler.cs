using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverviewQueryHandler(WhoIsHomeContext context, IDateTimeProvider dateTimeProvider)
{
    public async Task<UserOverview> HandleAsync(int userId, CancellationToken cancellationToken)
    {
        var eventList = await context.EventGroups
            .Include(e => e.Events)
            .Where(e => e.EndDate >= dateTimeProvider.CurrentDate)
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);

        var user = await context.Users
            .SingleAsync(u => u.Id == userId, cancellationToken);

        return new UserOverview
        {
            User = user,
            Events = eventList.Select(ToUserOverview).ToList()
        };
    }

    private UserOverviewEvent ToUserOverview(EventGroup eventGroup)
    {
        var nextDate = eventGroup.Events
            .Where(e => e.DeleteDate == null)
            .Where(e => e.Date >= dateTimeProvider.CurrentDate)
            .MinBy(e => e.Date)?.Date;

        nextDate ??= eventGroup.StartDate;
        
        return new UserOverviewEvent
        {
            GroupId = eventGroup.Id,
            Title = eventGroup.Title,
            NextDate = nextDate.Value,
            StartTime = eventGroup.StartTime,
            EndTime = eventGroup.EndTime,
            HasRepetitions = eventGroup.HasRepetitions
        };
    }
}