using Galaxus.Functional;
using Google.Cloud.Firestore;
using WhoIsHome.Services.Persons;

namespace WhoIsHome.Services.RepeatedEvents;

[FirestoreData]
public class RepeatedEvent
{
    [FirestoreDocumentId]
    public string? Id { get; set; }
    
    [FirestoreProperty] 
    public string EventName { get; set; } = null!;

    [FirestoreProperty] 
    public Person Person { get; set; } = null!;
    
    [FirestoreProperty]
    public Timestamp StartDate { get; set; }
    
    [FirestoreProperty]
    public Timestamp EndDate { get; set; }
    
    [FirestoreProperty]
    public Timestamp StartTime { get; set; }
    
    [FirestoreProperty]
    public Timestamp EndTime { get; set; }
    
    [FirestoreProperty]
    public bool RelevantForDinner { get; set; }
    
    [FirestoreProperty]
    public Timestamp? DinnerAt { get; set; }

    public bool IsAtHome => RelevantForDinner && DinnerAt != null;
    
    public bool IsToday => DateTime.Now.DayOfWeek != StartDate.ToDateTime().DayOfWeek;
    
    public static Result<RepeatedEvent, string> TryCreate(
        string eventName,
        Person person,
        DateTime startDate,
        DateTime endDate,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime? dinnerAt)
    {
        if (startTime >= endTime)
        {
            return $"{nameof(StartTime)} must be before {nameof(EndTime)}.";
        }
        
        if (startDate >= endDate)
        {
            return $"{nameof(StartDate)} must be before {nameof(EndDate)}.";
        }

        if (eventName.Length is <= 0 or >= 30)
        {
            return $"{nameof(EventName)} must be between 1 and 30 characters long.";
        }

        return new RepeatedEvent
        {
            Id = null,
            EventName = eventName,
            Person = person,
            StartDate = Timestamp.FromDateTime(startDate),
            EndDate = Timestamp.FromDateTime(endDate),
            StartTime = Timestamp.FromDateTime(startTime),
            EndTime = Timestamp.FromDateTime(endTime),
            RelevantForDinner = relevantForDinner,
            DinnerAt = dinnerAt.HasValue ? Timestamp.FromDateTime(dinnerAt.Value) : null
        };
    }

    public Result<Dictionary<string, object?>, string> TryUpdate(
        string eventName,
        DateTime startDate,
        DateTime endDate,
        DateTime startTime,
        DateTime endTime,
        bool relevantForDinner,
        DateTime? dinnerAt)
    {
        if (startTime >= endTime)
        {
            return $"{nameof(StartTime)} must be before {nameof(EndTime)}.";
        }

        if (startTime < DateTime.Today)
        {
            return "New Start Date can't be in the past.";
        }
        
        if (startDate >= endDate)
        {
            return $"{nameof(StartDate)} must be before {nameof(EndDate)}.";
        }

        if (eventName.Length is <= 0 or >= 30)
        {
            return $"{nameof(EventName)} must be between 1 and 30 characters long.";
        }

        EventName = eventName;
        StartDate = Timestamp.FromDateTime(startDate);
        EndDate = Timestamp.FromDateTime(endDate);
        StartTime = Timestamp.FromDateTime(startTime);
        EndTime = Timestamp.FromDateTime(endTime);
        RelevantForDinner = relevantForDinner;
        DinnerAt = dinnerAt.HasValue ? Timestamp.FromDateTime(dinnerAt.Value) : null;
        
        return new Dictionary<string, object?>
        {
            { nameof(EventName), EventName },
            { nameof(StartDate), StartDate },
            { nameof(EndDate), EndDate },
            { nameof(StartTime), StartTime },
            { nameof(EndTime), EndTime },
            { nameof(RelevantForDinner), RelevantForDinner },
            { nameof(DinnerAt), DinnerAt }
        };
    }
}