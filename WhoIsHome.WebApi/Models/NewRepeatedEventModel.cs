using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.WebApi.Models;

public class NewRepeatedEventModel
{
    public string EventName { get; set; } = null!;

    public string PersonId { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public bool RelevantForDinner { get; set; }
    
    public DateTime? DinnerAt { get; set; }
    
    public static NewRepeatedEventModel From(RepeatedEvent evenDbModel)
    {
        return new NewRepeatedEventModel
        {
            EventName = evenDbModel.EventName,
            PersonId = evenDbModel.Person.Id!,
            StartDate = evenDbModel.StartDate.ToDateTime(),
            EndDate = evenDbModel.EndDate.ToDateTime(),
            StartTime = evenDbModel.StartTime.ToDateTime(),
            EndTime = evenDbModel.EndTime.ToDateTime(),
            RelevantForDinner = evenDbModel.RelevantForDinner,
            DinnerAt = evenDbModel.DinnerAt?.ToDateTime()
        };
    }
}