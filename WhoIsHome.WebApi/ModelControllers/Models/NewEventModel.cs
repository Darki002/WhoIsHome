namespace WhoIsHome.WebApi.ModelControllers.Models;

public class NewEventModel
{
    public string EventName { get; set; } = null!;

    public string PersonId { get; set; } = null!;
    
    public DateOnly Date { get; set; }
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    public bool RelevantForDinner { get; set; }
    
    public TimeOnly? DinnerAt { get; set; }
}