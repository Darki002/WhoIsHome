using Galaxus.Functional;
using Google.Cloud.Firestore;
using WhoIsHome.Services.Events;
using WhoIsHome.Services.Persons;
using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverviewQueryHandler(
    IPersonService personService,
    IEventService eventService,
    IRepeatedEventService repeatedEventService)
{
    public async Task<Result<IReadOnlyCollection<PersonPresence>, string>> HandleAsync(CancellationToken cancellationToken)
    {
        var personsResult = await personService.GetAllAsync(cancellationToken);

        if (personsResult.IsErr) return personsResult.Err.Unwrap();

        var persons = personsResult.Unwrap();
        
        var today = Timestamp.FromDateTime(DateTime.UtcNow.Date);
        var tomorrow = Timestamp.FromDateTime(DateTime.UtcNow.Date.AddDays(1));

        var result = new List<PersonPresence>();
        
        foreach (var person in persons)
        {
            var events = await eventService.QueryManyAsync(cancellationToken,  async collectionRef =>
            {
                return await collectionRef
                    .WhereEqualTo("person:id", person.Id)
                    .WhereEqualTo("RelevantForDinner", true)
                    .WhereGreaterThanOrEqualTo("date", today)
                    .WhereLessThan("date", tomorrow)
                    .GetSnapshotAsync(cancellationToken);
            });

            if (events.Any(e => !e?.IsAtHome ?? false))
            {
                result.Add(PersonPresence.NotAtHome(person));
                continue;
            }
            
            var repeatedEvents = await repeatedEventService.QueryManyAsync(cancellationToken,  async collectionRef =>
            {
                return await collectionRef
                    .WhereEqualTo("person:id", person.Id)
                    .WhereEqualTo("RelevantForDinner", true)
                    .WhereLessThanOrEqualTo("firstDate", today)
                    .WhereGreaterThanOrEqualTo("lastDate", today)
                    .GetSnapshotAsync(cancellationToken);
            });
            
            if (repeatedEvents.Any(re => !re?.IsAtHome ?? false))
            {
                result.Add(PersonPresence.NotAtHome(person));
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

    private static PersonPresence GetPersonPresence(Event? e, RepeatedEvent? re, Person person)
    {
        if (e == null && re == null)
        {
            return PersonPresence.Empty(person);
        }
        
        if (EventIsBeforeRepeatedEventOrNoRepeatedEventIsGiven(e, re))
        {
            return PersonPresence.From(e!, person);
        }
    
        if (re != null)
        {
            return PersonPresence.From(re, person);
        }

        return PersonPresence.Empty(person);
    }

    private static bool EventIsBeforeRepeatedEventOrNoRepeatedEventIsGiven(Event? e, RepeatedEvent? re)
    {
        return e != null && (re == null || e.DinnerAt >= re.DinnerAt);
    }
}