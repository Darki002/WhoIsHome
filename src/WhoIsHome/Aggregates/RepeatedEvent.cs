using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Aggregates;

public class RepeatedEvent(
    int? id,
    string title,
    DateOnly firstOccurrence,
    DateOnly lastOccurrence,
    TimeOnly startTime,
    TimeOnly endTime,
    DinnerTime dinnerTime,
    int userId)
    : EventBase(id, title, startTime, endTime, dinnerTime, userId)
{
    private const int OccurrenceFrequency = 7;
    
    public DateOnly FirstOccurrence { get; set; } = firstOccurrence;

    public DateOnly LastOccurrence { get; set; } = lastOccurrence;

    public static RepeatedEvent Create(
        string title, 
        DateOnly firstOccurrence, 
        DateOnly lastOccurrence, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        PresenceType presenceType, 
        TimeOnly? time,
        int userId)
    {
        var dinnerTime = DinnerTime.Create(presenceType, time);
        ValidateBase(title, startTime, endTime, dinnerTime);
        ValidateOccurrence(firstOccurrence, lastOccurrence);
        
        return new RepeatedEvent(
            null,
            title,
            firstOccurrence,
            lastOccurrence,
            startTime,
            endTime,
            dinnerTime,
            userId);
    }
    
    public void Update(string title, DateOnly firstOccurrence, DateOnly lastOccurrence, TimeOnly startTime, TimeOnly endTime, PresenceType presenceType, TimeOnly? time)
    {
        var dinnerTime = DinnerTime.Update(presenceType, time);
        ValidateBase(title, startTime, endTime, dinnerTime);
        ValidateOccurrence(firstOccurrence, lastOccurrence);
        
        Title = title;
        FirstOccurrence = firstOccurrence;
        LastOccurrence = lastOccurrence;
        StartTime = startTime;
        EndTime = endTime;
        DinnerTime = dinnerTime;
    }

    public override bool IsEventAt(DateOnly date)
    {
        return DateTime.Now.DayOfWeek == FirstOccurrence.DayOfWeek &&
               date >= FirstOccurrence &&
               date <= LastOccurrence;
    }

    public override DateOnly GetNextOccurrence(DateOnly date)
    {
        if (date > LastOccurrence)
        {
            throw new InvalidOperationException("Can't get the next occurrence of an Event that is in the past.");
        }

        if (FirstOccurrence > date)
        {
            return FirstOccurrence;
        }

        if (IsEventAt(date))
        {
            return date;
        }
        
        var daysUntilNextOccurence = date.DaysUntilNext(FirstOccurrence.DayOfWeek);
        return date.AddDays(daysUntilNextOccurence);
    }

    private static void ValidateOccurrence(DateOnly firstOccurrence,  DateOnly lastOccurrence)
    {
        if (firstOccurrence > lastOccurrence)
        {
            throw new InvalidModelException("First occurrence must be before the last occurrence.");
        }
    }
}