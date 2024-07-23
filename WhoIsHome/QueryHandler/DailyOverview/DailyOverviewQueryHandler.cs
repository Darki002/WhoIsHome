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
        
        var today = Timestamp.FromDateTime(DateTime.Today);
        var tomorrow = Timestamp.FromDateTime(DateTime.Today.AddDays(1));

        var result = new List<PersonPresence>();
        
        foreach (var person in persons)
        {
            // Won't work. We have to get them all. In case there is one that has RelevantForDinner but no DinnerAt -> NotAtHome at all
            var latestDinnerAtEvent = await eventService.QuerySingleAsync(cancellationToken,  async collectionRef =>
            {
                return await collectionRef
                    .WhereEqualTo("person:id", person.Id)
                    .WhereEqualTo("RelevantForDinner", true)
                    .WhereGreaterThanOrEqualTo("date", today)
                    .WhereLessThan("date", tomorrow)
                    .OrderByDescending("dinnerAt")
                    .Limit(1)
                    .GetSnapshotAsync(cancellationToken);
            });
            
            var repeatedEvents = await repeatedEventService.QueryManyAsync(cancellationToken,  async collectionRef =>
            {
                return await collectionRef
                    .WhereEqualTo("person:id", person.Id)
                    .WhereEqualTo("RelevantForDinner", true)
                    .WhereLessThanOrEqualTo("firstDate", today)
                    .WhereGreaterThanOrEqualTo("lastDate", today)
                    .GetSnapshotAsync(cancellationToken);
            });

            // Won't work. We have to get them all. In case there is one that has RelevantForDinner but no DinnerAt -> NotAtHome at all
            var latestRepeatedEvent = repeatedEvents
                .Where(re => re != null)
                .Where(re => re!.IsToday)
                .MaxBy(re => re!.DinnerAt);

            var personPresence = GetPersonPresence(latestDinnerAtEvent, latestRepeatedEvent, person);
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