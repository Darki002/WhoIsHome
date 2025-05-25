using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Aggregates;

public class OneTimeEvent(
    int? id,
    string title,
    DateOnly date,
    TimeOnly startTime,
    TimeOnly? endTime,
    DinnerTime dinnerTime,
    int userId)
    : EventBase(id, title, startTime, endTime, dinnerTime, userId)
{
    public DateOnly Date { get; private set; } = date;

    public static OneTimeEvent Create(
        string title, 
        DateOnly date, 
        TimeOnly startTime, 
        TimeOnly? endTime, 
        PresenceType presenceType, 
        TimeOnly? time,
        int userId)
    {
        var dinnerTime = DinnerTime.Create(presenceType, time);
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
    
    public void Update(string title, DateOnly date, TimeOnly startTime, TimeOnly? endTime, PresenceType presenceType, TimeOnly? time)
    {
        var dinnerTime = DinnerTime.Update(presenceType, time);
        ValidateBase(title, startTime, endTime, dinnerTime);
        
        Title = title;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        DinnerTime = dinnerTime;
    }

    public override bool IsEventAt(DateOnly date) => Date == date;

    public override DateOnly GetNextOccurrence(DateOnly today) => Date;
}