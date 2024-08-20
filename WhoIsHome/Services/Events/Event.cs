using Galaxus.Functional;
using Google.Cloud.Firestore;
using WhoIsHome.Services.Persons;

namespace WhoIsHome.Services.Events;

[FirestoreData]
public class Event
{
    [FirestoreDocumentId]
    public string? Id { get; set; }

    [FirestoreProperty] 
    public string EventName { get; set; } = null!;

    [FirestoreProperty] 
    public Person Person { get; set; } = null!;
    
    [FirestoreProperty]
    public Timestamp Date { get; set; }
    
    [FirestoreProperty]
    public int StartTime { get; set; }
    
    [FirestoreProperty]
    public int EndTime { get; set; }
    
    [FirestoreProperty]
    public bool RelevantForDinner { get; set; }
    
    [FirestoreProperty]
    public int? DinnerAt { get; set; }

    public bool IsAtHome => RelevantForDinner && DinnerAt != null;

    public bool IsToday => Date.ToDateTime() == DateTime.Now.Date;

    public static Result<Event, string> TryCreate(
        string eventName,
        Person person,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt)
    {
        if (startTime >= endTime)
        {
            return $"{nameof(StartTime)} must be before {nameof(EndTime)}.";
        }

        if (eventName.Length is <= 0 or >= 30)
        {
            return $"{nameof(EventName)} must be between 1 and 30 characters long.";
        }
        
        return new Event
        {
            Id = null,
            EventName = eventName,
            Person = person,
            Date = date.ToTimespan(),
            StartTime = startTime.ToSeconds(),
            EndTime = endTime.ToSeconds(),
            RelevantForDinner = relevantForDinner,
            DinnerAt = dinnerAt?.ToSeconds()
        };
    }

    public Result<Dictionary<string, object?>, string> TryUpdate(
        string eventName,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt)
    {
        if (startTime >= endTime)
        {
            return $"{nameof(StartTime)} must be before {nameof(EndTime)}.";
        }

        if (date < DateOnly.FromDateTime(DateTime.Now))
        {
            return "New Date can't be in the past.";
        }

        if (eventName.Length is <= 0 or >= 30)
        {
            return $"{nameof(EventName)} must be between 1 and 30 characters long.";
        }

        EventName = eventName;
        Date = date.ToTimespan();
        StartTime = startTime.ToSeconds();
        EndTime = endTime.ToSeconds();
        RelevantForDinner = relevantForDinner;
        DinnerAt = dinnerAt?.ToSeconds();
        
        return new Dictionary<string, object?>
        {
            { nameof(EventName), EventName },
            { nameof(Date), Date },
            { nameof(StartTime), StartTime },
            { nameof(EndTime), EndTime },
            { nameof(RelevantForDinner), RelevantForDinner },
            { nameof(DinnerAt), DinnerAt }
        };
    }
}