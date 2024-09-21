using WhoIsHome.Shared;

namespace WhoIsHome.Aggregates;

public class RepeatedEvent : EventBase
{
    private const int OccurrenceFrequency = 7;
    
    public DateOnly FirstOccurrence { get; set; }
    
    public DateOnly LastOccurrence { get; set; }
    
    private RepeatedEvent(
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
        
        if (firstOccurrence > lastOccurrence)
        {
            throw new ArgumentException("First occurrence must be before the last occurrence.", nameof(firstOccurrence));
        }
        
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
        
        var daysLeftThisWeek = 7 - (int)today.DayOfWeek;

        var occurence = today.AddDays(daysLeftThisWeek).AddDays((int)FirstOccurrence.DayOfWeek);
        return occurence;
    }
}