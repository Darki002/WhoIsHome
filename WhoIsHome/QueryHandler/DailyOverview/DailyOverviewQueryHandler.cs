using Galaxus.Functional;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverviewQueryHandler(WhoIsHomeContext context)
{
    public async Task<Result<IReadOnlyCollection<DailyOverview>, string>> HandleAsync(CancellationToken cancellationToken)
    {
        var users = await context.Users.ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.Today);

        var result = new List<DailyOverview>();

        var events = (await context.Events
                .Where(e => e.DinnerTimeModel.PresentsType != PresentsType.Unknown)
                .Where(e => e.Date == today)
                .GroupBy(e => e.UserModel.Id)
                .ToListAsync(cancellationToken))
            .Select(g => g.ToModelList<Event, EventModel>())
            .ToList();
        
        var repeatedEvents = (await context.RepeatedEvents
                .Where(e => e.DinnerTimeModel.PresentsType != PresentsType.Unknown)
                .Where(e => e.FirstOccurrence > today)
                .Where(e => e.LastOccurrence <= today)
                .GroupBy(e => e.UserModel.Id)
                .ToListAsync(cancellationToken))
            .Select(g => g.ToModelList<RepeatedEvent, RepeatedEventModel>())
            .ToList();
        
        // TODO Use a base Type Event for Repeated and OneTime. The base type has all the necessary things to calculate the Overview and other stuff
        
        foreach (var user in users)
        {
            if (events.Any(e => !e?.IsAtHome ?? false))
            {
                result.Add(DailyOverview.NotAtHome(person));
                continue;
            }
            
            if (repeatedEvents.Any(re => !re?.IsAtHome ?? false))
            {
                result.Add(DailyOverview.NotAtHome(person));
                continue;
            }

            var latestEvent = events
                .Where(re => re != null)
                .Where(re => re!.IsToday)
                .MaxBy(re => re!.DinnerAt);
            
            var latestRepeatedEvent = repeatedEvents
                .Where(re => re != null)
                .Where(re => re!.IsToday)
                .MaxBy(re => re!.DinnerAt);

            var personPresence = GetPersonPresence(latestEvent, latestRepeatedEvent, person);
            result.Add(personPresence);
        }

        return result;
    }

    private static DailyOverview GetPersonPresence(Event? e, RepeatedEvent? re, User user)
    {
        if (e == null && re == null)
        {
            return DailyOverview.Empty(user);
        }
        
        if (EventIsBeforeRepeatedEventOrNoRepeatedEventIsGiven(e, re))
        {
            return DailyOverview.From(user, e!.DinnerTime);
        }
    
        return re != null ? DailyOverview.From(user, re.DinnerTime) : DailyOverview.Empty(user);
    }

    private static bool EventIsBeforeRepeatedEventOrNoRepeatedEventIsGiven(Event? e, RepeatedEvent? re)
    {
        return e != null && (re == null || e.DinnerTime.Time >= re.DinnerTime.Time);
    }
}