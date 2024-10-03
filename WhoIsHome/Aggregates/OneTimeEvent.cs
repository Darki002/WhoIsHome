namespace WhoIsHome.Aggregates;

public class OneTimeEvent : EventBase
{
    public DateOnly Date { get; private set; }

    public OneTimeEvent(int? id, string title, DateOnly date, TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime,
        int userId) : base(id, title, startTime, endTime, dinnerTime, userId)
    {
        Date = date;
    }
    
    public static OneTimeEvent Create(
        string title, 
        DateOnly date, 
        TimeOnly startTime, 
        TimeOnly endTime, 
        DinnerTime dinnerTime,
        int userId)
    {
        ValidateBase(title, startTime, endTime, dinnerTime);
        
        return new OneTimeEvent(
            null,
            title,
            date,
            startTime,
            endTime,
            dinnerTime,
            userId);
    }
    
    public void Update(string title, DateOnly date, TimeOnly startTime, TimeOnly endTime, DinnerTime dinnerTime)
    {
        ValidateBase(title, startTime, endTime, dinnerTime);
        
        Title = title;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        DinnerTime = DinnerTime.Update(dinnerTime.PresentsType, dinnerTime.Time);
    }
    
    protected override bool IsEventToday() => Date == DateOnly.FromDateTime(DateTime.Today);

    public override DateOnly GetNextOccurrence() => Date;
}