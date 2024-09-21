using WhoIsHome.Shared;

namespace WhoIsHome.Aggregates;

public abstract class EventBase(
    int? id,
    string title,
    TimeOnly startTime,
    TimeOnly endTime,
    DinnerTime dinnerTime,
    int userId)
    : AggregateBase
{
    public int? Id { get; } = id;

    public string Title { get; protected set; } = title;

    public TimeOnly StartTime { get; protected set; } = startTime;

    public TimeOnly EndTime { get; protected set; } = endTime;

    public DinnerTime DinnerTime { get; protected set; } = dinnerTime;

    public int UserId { get; } = userId;

    public bool IsToday => IsEventToday();

    protected static void ValidateBase(
        string title,
        TimeOnly startTime,
        TimeOnly endTime,
        DinnerTime dinnerTime)
    {
        ValidateTitle(title);
        ValidateTime(startTime, endTime);
        ValidateDinnerTime(endTime, dinnerTime);
    }
    
    private static void ValidateTitle(string title)
    {
        if (title.Length >= 50)
        {
            throw new ArgumentException("Title must be less then or equal to 50 characters long.", nameof(title));
        }
    }

    private static void ValidateTime(TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime > endTime)
        {
            throw new ArgumentException("StartDate must be before EndDate.", nameof(startTime));
        }
    }

    private static void ValidateDinnerTime(TimeOnly endTime, DinnerTime dinnerTime)
    {
        if (endTime > dinnerTime.Time)
        {
            throw new ArgumentException("Dinner Time must be later then the End Time of the Event.", nameof(dinnerTime));
        }
    }

    protected abstract bool IsEventToday();

    public abstract DateOnly GetNextOccurrence();
}