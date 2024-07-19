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

        foreach (var person in persons)
        {
            var latestDinnerAtEvent = eventService.QuerySingleAsync(cancellationToken,  async collectionRef =>
            {
                return await collectionRef
                    .WhereEqualTo("person:id", person.Id)
                    .WhereGreaterThanOrEqualTo("date", today)
                    .WhereLessThan("date", tomorrow)
                    .OrderByDescending("dinnerAt")
                    .Limit(1)
                    .GetSnapshotAsync(cancellationToken);
            });
            
            var todayRepeatedEvents = repeatedEventService.QueryManyAsync(cancellationToken,  async collectionRef =>
            {
                return await collectionRef
                    .WhereEqualTo("person:id", person.Id)
                    .WhereLessThanOrEqualTo("firstDate", today)
                    .WhereGreaterThanOrEqualTo("lastDate", today)
                    .GetSnapshotAsync(cancellationToken);
            });
        }

        return null!;
    }
}