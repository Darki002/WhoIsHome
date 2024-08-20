using Galaxus.Functional;
using Google.Cloud.Firestore;
using WhoIsHome.Services.Persons;

namespace WhoIsHome.Services.RepeatedEvents;

[FirestoreData]
public class RepeatedEvent
{
    [FirestoreDocumentId] public string? Id { get; set; }

    [FirestoreProperty] public string EventName { get; set; } = null!;

    [FirestoreProperty] public Person Person { get; set; } = null!;

    [FirestoreProperty] public Timestamp StartDate { get; set; }

    [FirestoreProperty] public Timestamp EndDate { get; set; }

    [FirestoreProperty] public int StartTime { get; set; }

    [FirestoreProperty] public int EndTime { get; set; }

    [FirestoreProperty] public bool RelevantForDinner { get; set; }

    [FirestoreProperty] public int? DinnerAt { get; set; }

    public bool IsAtHome => RelevantForDinner && DinnerAt != null;

    public bool IsToday => DateTime.Now.DayOfWeek != StartDate.ToDateTime().DayOfWeek &&
                           DateTime.Now.Date < EndDate.ToDateTime().Date;

    public static Result<RepeatedEvent, string> TryCreate(
        string eventName,
        Person person,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt)
    {
        if (startTime >= endTime) return $"{nameof(StartTime)} must be before {nameof(EndTime)}.";

        if (startDate >= endDate) return $"{nameof(StartDate)} must be before {nameof(EndDate)}.";
        
        if (startDate < DateOnly.FromDateTime(DateTime.Now)) return "New Date can't be in the past.";

        if (eventName.Length is <= 0 or >= 30) return $"{nameof(EventName)} must be between 1 and 30 characters long.";

        return new RepeatedEvent
        {
            Id = null,
            EventName = eventName,
            Person = person,
            StartDate = startDate.ToTimespan(),
            EndDate = endDate.ToTimespan(),
            StartTime = startTime.ToSeconds(),
            EndTime = endTime.ToSeconds(),
            RelevantForDinner = relevantForDinner, 
            DinnerAt = dinnerAt?.ToSeconds()
        };
    }

    public Result<Dictionary<string, object?>, string> TryUpdate(
        string eventName,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly startTime,
        TimeOnly endTime,
        bool relevantForDinner,
        TimeOnly? dinnerAt)
    {
        if (startTime >= endTime) return $"{nameof(StartTime)} must be before {nameof(EndTime)}.";

        if (startDate < DateOnly.FromDateTime(DateTime.Now)) return "New Start Date can't be in the past.";

        if (startDate >= endDate) return $"{nameof(StartDate)} must be before {nameof(EndDate)}.";

        if (eventName.Length is <= 0 or >= 30) return $"{nameof(EventName)} must be between 1 and 30 characters long.";

        EventName = eventName;
        StartDate = startDate.ToTimespan();
        EndDate = endDate.ToTimespan();
        StartTime = startTime.ToSeconds();
        EndTime = endTime.ToSeconds();
        RelevantForDinner = relevantForDinner;
        DinnerAt = dinnerAt?.ToSeconds();

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

    public DateOnly? GetNextOccurrence()
    {
        var now = DateTime.Now;
        var startDate = StartDate.ToDateTime();
        var occurrence = DateTime.Now.Date;
        
        if (EndDate.ToDateTime() < now) {return null;}

        if (startDate > now) return StartDate.ToDateOnly();

        if ((int)now.DayOfWeek > (int)startDate.DayOfWeek)
        {
            var dist = (int)DayOfWeek.Saturday - (int)now.DayOfWeek;
            occurrence = occurrence.AddDays(dist);
        }
        return DateOnly.FromDateTime(occurrence.AddDays((int)startDate.DayOfWeek - (int)now.DayOfWeek));
    }
}