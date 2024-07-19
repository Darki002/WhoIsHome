using WhoIsHome.Services.Events;
using WhoIsHome.Services.Persons;
using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverviewQueryHandler(
    IPersonService personService, 
    IEventService eventService, 
    IRepeatedEventService repeatedEventService)
{
    public IReadOnlyCollection<PersonPresence> HandleAsync(CancellationToken cancellationToken)
    {
        return null!;
    }
}