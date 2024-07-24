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
        
        return null!;
    }
}