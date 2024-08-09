using Galaxus.Functional;
using Google.Cloud.Firestore;
using WhoIsHome.Services.Events;
using WhoIsHome.Services.Persons;
using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.QueryHandler.PersonOverview;

public class PersonOverviewQueryHandler(
    IPersonService personService,
    IEventService eventService,
    IRepeatedEventService repeatedEventService)
{
    public async Task<Result<PersonOverview, string>> HandleAsync(string personId, CancellationToken cancellationToken)
    {
        var personResult = await personService.GetAsync(personId, cancellationToken);
        if (personResult.IsErr) return personResult.Err.Unwrap();
        var person = personResult.Unwrap()!;

        var today = Timestamp.FromDateTime(DateTime.UtcNow.Date);
        
        var events = await eventService.QueryManyAsync(cancellationToken, query =>
        {
            return query.WherePersonIs(personId)
                .WhereGreaterThanOrEqualTo("Date", today)
                .GetSnapshotAsync(cancellationToken);
        });
        
        var repeatedEvents = await repeatedEventService.QueryManyAsync(cancellationToken, query =>
        {
            return query.WherePersonIs(personId)
                .WhereGreaterThanOrEqualTo("EndDate", today)
                .GetSnapshotAsync(cancellationToken);
        });

        var result = events
            .Select(e => new PersonOverviewEvent
            {
                Id = e.Id!,
                EventName = e.EventName,
                Date = e.Date.ToDateTime(),
                StartTime = e.StartTime.ToDateTime(),
                EndTime = e.EndTime.ToDateTime(),
                EventType = EventType.Event
            })
            .ToList();
        
        result.AddRange(
            repeatedEvents
                .Select(re => (Event: re, NextOccurrence: re.GetNextOccurrence()))
                .Where(re => re.NextOccurrence.HasValue)
                .Select(re => new PersonOverviewEvent
                {
                    Id = re.Event.Id!,
                    EventName = re.Event.EventName,
                    Date = re.NextOccurrence!.Value,
                    StartTime = re.Event.StartTime.ToDateTime(),
                    EndTime = re.Event.EndTime.ToDateTime(),
                    EventType = EventType.RepeatedEvent
                })
            );

        return new PersonOverview
        {
            Person = person,
            Events = result
        };
    }
}