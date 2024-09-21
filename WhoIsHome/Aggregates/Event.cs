using WhoIsHome.Shared;

namespace WhoIsHome.Aggregates;

public class Event : Aggregate
{
    public int? Id { get; }

    public string Title { get; private set; }

    public DateOnly Date { get; private set; }

    public TimeOnly StartTime { get; private set; }

    public TimeOnly EndTime { get; private set; }

    public DinnerTime DinnerTime { get; }

    public int UserId { get; }
    
    
    public bool IsToday => Date == DateOnly.FromDateTime(DateTime.Today);
    
    private Event(int? id, string title, DateOnly date, TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime,
        int userId)
    {
        Id = id;
        Title = title;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        DinnerTime = dinnerTime;
        UserId = userId;
    }
    
    public static Event Create(
        string title, 
        DateOnly date, 
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

        if (endTime > dinnerTime.Time)
        {
            throw new ArgumentException("Dinner Time must be later then the End Time of the Event.", nameof(dinnerTime));
        }
        
        return new Event(
            null,
            title,
            date,
            startTime,
            endTime,
            dinnerTime,
            userId);
    }
}