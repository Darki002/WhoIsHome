using Google.Cloud.Firestore;
using WhoIsHome.Services.Events;
using WhoIsHome.Services.Persons;
using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class PersonPresence
{
    public Person Person { get; set; }

    public bool IsAtHome { get; set; } = true;

    public TimeOnly? DinnerAt { get; set; }

    public static PersonPresence Empty(Person person)
    {
        return new PersonPresence
        {
            Person = person
        };
    }

    public static PersonPresence From(Event @event, Person person)
    {
        var dinnerAtTime = TryGetTimeOnly(@event.DinnerAt);
        
        return new PersonPresence
        {
            Person = person,
            IsAtHome = @event.IsAtHome,
            DinnerAt = dinnerAtTime
        };
    }

    public static PersonPresence From(RepeatedEvent repeatedEvent, Person person)
    {
        var dinnerAtTime = TryGetTimeOnly(repeatedEvent.DinnerAt);
        
        return new PersonPresence
        {
            Person = person,
            IsAtHome = repeatedEvent.IsAtHome,
            DinnerAt = dinnerAtTime
        };
    }
    
    private static TimeOnly? TryGetTimeOnly(Timestamp? timestamp)
    {
        return timestamp != null 
            ? TimeOnly.FromDateTime(timestamp!.Value.ToDateTime()) 
            : null;
    }
}