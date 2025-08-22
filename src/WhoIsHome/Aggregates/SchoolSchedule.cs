using WhoIsHome.Shared.Exceptions;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Aggregates;

public class SchoolSchedule(
    int? id,
    string schoolName,
    int dayOfWeek,
    TimeOnly startTime,
    TimeOnly? endTime,
    DinnerTime? dinnerTime,
    int userId)
{
    public int? Id { get; init; } = id;
    public string SchoolName { get; private set; } = schoolName;
    public int DayOfWeek { get; private set; } = dayOfWeek;
    public TimeOnly StartTime { get; private set; } = startTime;
    public TimeOnly? EndTime { get; private set; } = endTime;
    public DinnerTime? DinnerTime { get; private set; } = dinnerTime;
    public int UserId { get; init; } = userId;

    public static SchoolSchedule Create(
        string title, 
        int dayOfWeek,
        TimeOnly startTime, 
        TimeOnly? endTime, 
        PresenceType presenceType, 
        TimeOnly? time,
        int userId)
    {
        var dinnerTime = DinnerTime.Create(presenceType, time);
        Validate(title, startTime, endTime, dinnerTime);
        
        return new SchoolSchedule(
            null,
            title,
            dayOfWeek,
            startTime,
            endTime,
            dinnerTime,
            userId);
    }
    
    public void Update(string title, int dayOfWeek, TimeOnly startTime, TimeOnly? endTime, PresenceType presenceType, TimeOnly? time)
    {
        var dinnerTime = DinnerTime?.Update(presenceType, time);
        Validate(title, startTime, endTime, dinnerTime);
        
        SchoolName = title;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        DinnerTime = dinnerTime;
    }
    
    private static void Validate(
        string schoolName,
        TimeOnly startTime,
        TimeOnly? endTime,
        DinnerTime? dinnerTime)
    {
        ValidateSchoolName(schoolName);
        ValidateTime(startTime, endTime);
        if(dinnerTime is not null) ValidateDinnerTime(endTime, dinnerTime);
    }
    
    private static void ValidateSchoolName(string schoolName)
    {
        if (schoolName.Length >= 50)
        {
            throw new InvalidModelException("Title must be less then or equal to 50 characters long.");
        }
    }

    private static void ValidateTime(TimeOnly startTime, TimeOnly? endTime)
    {
        if (startTime > endTime)
        {
            throw new InvalidModelException("StartDate must be before EndDate.");
        }
    }

    private static void ValidateDinnerTime(TimeOnly? endTime, DinnerTime dinnerTime)
    {
        if (endTime > dinnerTime.Time)
        {
            throw new InvalidModelException("Dinner Time must be later then the End Time of the Event.");
        }
    }
}