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
    public Timestamp StartTime { get; set; }
    
    [FirestoreProperty]
    public Timestamp EndTime { get; set; }
    
    [FirestoreProperty]
    public bool RelevantForDinner { get; set; }
    
    [FirestoreProperty]
    public Timestamp? DinnerAt { get; set; }

    public bool IsAtHome => RelevantForDinner && DinnerAt != null;

    public bool IsToday => Date.ToDateTime() == DateTime.Today;

    public static Result<Event, string> TryCreate(
        string eventName,
        Person person,
        DateTime date,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime? dinnerAt)
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
            Date = Timestamp.FromDateTime(date.Date),
            StartTime = Timestamp.FromDateTime(startTime),
            EndTime = Timestamp.FromDateTime(endTime),
            RelevantForDinner = relevantForDinner,
            DinnerAt = dinnerAt.HasValue ? Timestamp.FromDateTime(dinnerAt.Value) : null
        };
    }

    public Result<Dictionary<string, object?>, string> TryUpdate(
        string eventName,
        DateTime date,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime? dinnerAt)
    {
        if (startTime >= endTime)
        {
            return $"{nameof(StartTime)} must be before {nameof(EndTime)}.";
        }

        if (date < DateTime.Today)
        {
            return "New Date can't be in the past.";
        }

        if (eventName.Length is <= 0 or >= 30)
        {
            return $"{nameof(EventName)} must be between 1 and 30 characters long.";
        }

        EventName = eventName;
        Date = Timestamp.FromDateTime(date.Date);
        StartTime = Timestamp.FromDateTime(startTime);
        EndTime = Timestamp.FromDateTime(endTime);
        RelevantForDinner = relevantForDinner;
        DinnerAt = dinnerAt.HasValue ? Timestamp.FromDateTime(dinnerAt.Value) : null;
        
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