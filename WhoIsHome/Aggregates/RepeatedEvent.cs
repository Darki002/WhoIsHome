using WhoIsHome.Shared;
using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Aggregates;

public class RepeatedEvent : EventBase
{
    private const int OccurrenceFrequency = 7;
    
    public DateOnly FirstOccurrence { get; set; }
    
    public DateOnly LastOccurrence { get; set; }
    
    public RepeatedEvent(
        int? id, 
        string title, 
        DateOnly firstOccurrence, 
        DateOnly lastOccurrence, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        DinnerTime dinnerTime, 
        int userId) : base(id, title, startTime, endTime, dinnerTime, userId)
    {
        FirstOccurrence = firstOccurrence;
        LastOccurrence = lastOccurrence;
    }
    
    public static RepeatedEvent Create(
        string title, 
        DateOnly firstOccurrence, 
        DateOnly lastOccurrence, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        DinnerTime dinnerTime, 
        int userId)
    {
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
    
    public void Update(string title, DateOnly firstOccurrence, DateOnly lastOccurrence, TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime)
    {
        ValidateBase(title, startTime, endTime, dinnerTime);
        ValidateOccurrence(firstOccurrence, lastOccurrence);
        
        Title = title;
        FirstOccurrence = firstOccurrence;
        LastOccurrence = lastOccurrence;
        StartTime = startTime;
        EndTime = endTime;
        DinnerTime = DinnerTime.Update(dinnerTime.PresentsType, dinnerTime.Time);
    }
    
    protected override bool IsEventToday()
    {
        return DateTime.Now.DayOfWeek != FirstOccurrence.DayOfWeek &&
               DateOnly.FromDateTime(DateTime.Today) < LastOccurrence;
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
        
        var daysLeftThisWeek = OccurrenceFrequency - (int)today.DayOfWeek;

        var occurence = today.AddDays(daysLeftThisWeek).AddDays((int)FirstOccurrence.DayOfWeek);
        return occurence;
    }

    private static void ValidateOccurrence(DateOnly firstOccurrence,  DateOnly lastOccurrence)
    {
        if (firstOccurrence > lastOccurrence)
        {
            throw new InvalidModelException("First occurrence must be before the last occurrence.");
        }
    }
}