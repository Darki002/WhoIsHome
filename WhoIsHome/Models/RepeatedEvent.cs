namespace WhoIsHome.Models;

public class RepeatedEvent
{
    public int? Id { get; set; }
    
    public string Title { get; set; }
    
    public DateOnly FirstOccurrence { get; set; }
    
    public DateOnly LastOccurrence { get; set; }
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    public DinnerTime DinnerTime { get; set; }
    
    public int UserId { get; set; }

    private RepeatedEvent(
        int? id, 
        string title, 
        DateOnly firstOccurrence, 
        DateOnly lastOccurrence, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        DinnerTime dinnerTime, 
        int userId)
    {
        Id = id;
        Title = title;
        FirstOccurrence = firstOccurrence;
        LastOccurrence = lastOccurrence;
        StartTime = startTime;
        EndTime = endTime;
        DinnerTime = dinnerTime;
        UserId = userId;
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
        if (title.Length >= 50)
        {
            throw new ArgumentException("Title must be less then or equal to 50 characters long.", nameof(title));
        }

        if (startTime > endTime)
        {
            throw new ArgumentException("StartDate must be before EndDate.", nameof(startTime));
        }
        
        if (firstOccurrence > lastOccurrence)
        {
            throw new ArgumentException("First occurrence must be before the last occurrence.", nameof(firstOccurrence));
        }
        
        if (endTime > dinnerTime.Time)
        {
            throw new ArgumentException("Dinner Time must be later then the End Time of the Event.", nameof(dinnerTime));
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
}