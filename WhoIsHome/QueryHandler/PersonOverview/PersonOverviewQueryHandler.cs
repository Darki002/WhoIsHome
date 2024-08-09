using Galaxus.Functional;
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

        var events = await eventService.GetByPersonIdAsync(personId, cancellationToken);
        var repeatedEvents = await repeatedEventService.GetByPersonIdAsync(personId, cancellationToken);

        if (events.IsErr) return events.Err.Unwrap();
        if (repeatedEvents.IsErr) return repeatedEvents.Err.Unwrap();

        var result = events.Unwrap()
            .Select(e => new PersonOverviewEvent
            {
                Id = e.Id!,
                EventName = e.EventName,
                Date = e.Date.ToDateTime(),
                StartTime = e.StartTime.ToDateTime(),
                EndTime = e.EndTime.ToDateTime()
            })
            .ToList();
        
        result.AddRange(
            repeatedEvents.Unwrap()
                .Select(re => (Event: re, NextOccurrence: re.GetNextOccurrence()))
                .Where(re => re.NextOccurrence.HasValue)
                .Select(re => new PersonOverviewEvent
                {
                    Id = re.Event.Id!,
                    EventName = re.Event.EventName,
                    Date = re.NextOccurrence!.Value,
                    StartTime = re.Event.StartTime.ToDateTime(),
                    EndTime = re.Event.EndTime.ToDateTime()
                })
            );

        return new PersonOverview
        {
            Person = person,
            Events = result
        };
    }
}