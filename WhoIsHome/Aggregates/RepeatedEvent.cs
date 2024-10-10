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
    
    protected override bool IsEventToday()
    {
        return DateTime.Now.DayOfWeek == FirstOccurrence.DayOfWeek &&
               DateOnly.FromDateTime(DateTime.Today) <= LastOccurrence;
    }

    public override DateOnly GetNextOccurrence()
    {
        var today = DateOnlyHelper.Today;

        if (today > LastOccurrence)
        {
            throw new InvalidOperationException("Can't get the next occurrence of an Event that is in the past.");
        }

        if (FirstOccurrence > today)
        {
            return FirstOccurrence;
        }

        if (IsToday)
        {
            return today;
        }

        if (firstOccurrence.IsSameWeek(today))
        {
            var daysUntilNextOccurence = (int)firstOccurrence.DayOfWeek - (int)today.DayOfWeek;
            return today.AddDays(daysUntilNextOccurence).AddDays((int)FirstOccurrence.DayOfWeek);
        }
        
        var daysLeftThisWeek = OccurrenceFrequency - (int)today.DayOfWeek;
        return today.AddDays(daysLeftThisWeek).AddDays((int)FirstOccurrence.DayOfWeek);
    }

    private static void ValidateOccurrence(DateOnly firstOccurrence,  DateOnly lastOccurrence)
    {
        if (firstOccurrence > lastOccurrence)
        {
            throw new InvalidModelException("First occurrence must be before the last occurrence.");
        }
    }
}