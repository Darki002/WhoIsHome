namespace WhoIsHome.WebApi.ModelControllers.Models;

public class NewEventModel
{
    public string EventName { get; set; } = null!;

    public string PersonId { get; set; } = null!;
    
    public DateTime Date { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public bool RelevantForDinner { get; set; }
    
    public DateTime? DinnerAt { get; set; }
}