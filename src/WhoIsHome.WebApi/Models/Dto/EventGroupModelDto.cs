namespace WhoIsHome.WebApi.Models.Dto;

public class EventGroupModelDto
{
    public string Title { get; set; } = null!;
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly? EndDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    public List<DayOfWeek> WeekDays { get; set; } = [];

    public string PresenceType { get; set; } = null!;

    public TimeOnly? DinnerTime { get; set; } = null;
}