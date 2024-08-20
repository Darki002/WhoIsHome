using Google.Cloud.Firestore;
using WhoIsHome.Services.Events;
using WhoIsHome.Services.Persons;
using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverview
{
    public required Person Person { get; init; }

    public bool IsAtHome { get; init; } = true;

    public TimeOnly? DinnerAt { get; init; }

    public static DailyOverview Empty(Person person)
    {
        return new DailyOverview
        {
            Person = person
        };
    }

    public static DailyOverview NotAtHome(Person person)
    {
        return new DailyOverview
        {
            Person = person,
            IsAtHome = false,
            DinnerAt = null
        };
    }

    public static DailyOverview From(Event @event, Person person)
    {
        var dinnerAtTime = TryGetTimeOnly(@event.DinnerAt);
        
        return new DailyOverview
        {
            Person = person,
            IsAtHome = @event.IsAtHome,
            DinnerAt = dinnerAtTime
        };
    }

    public static DailyOverview From(RepeatedEvent repeatedEvent, Person person)
    {
        var dinnerAtTime = TryGetTimeOnly(repeatedEvent.DinnerAt);
        
        return new DailyOverview
        {
            Person = person,
            IsAtHome = repeatedEvent.IsAtHome,
            DinnerAt = dinnerAtTime
        };
    }
    
    private static TimeOnly? TryGetTimeOnly(int? timestamp) => timestamp?.ToTimeOnly();
}