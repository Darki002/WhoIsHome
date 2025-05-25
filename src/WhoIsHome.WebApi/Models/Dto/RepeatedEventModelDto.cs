namespace WhoIsHome.WebApi.Models.Dto;

public class RepeatedEventModelDto
{
    public required string Title { get; set; }
    
    public required DateOnly FirstOccurrence { get; set; }
    
    public DateOnly? LastOccurrence { get; set; }

    public required TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public required string PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;
}