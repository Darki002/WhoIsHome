namespace WhoIsHome.WebApi.Models.Dto;

public class EventInstanceDto
{
    public DateOnly Date { get; set; }
    
    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    public string PresenceType { get; set; } = null!;

    public TimeOnly? DinnerTime { get; set; } = null;
}