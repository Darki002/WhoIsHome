using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.WebApi.ModelControllers.Models;

public class NewRepeatedEventModel
{
    public string EventName { get; set; } = null!;

    public string PersonId { get; set; } = null!;
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    public bool RelevantForDinner { get; set; }
    
    public TimeOnly? DinnerAt { get; set; }
    
    public static NewRepeatedEventModel From(RepeatedEvent evenDbModel)
    {
        return new NewRepeatedEventModel
        {
            EventName = evenDbModel.EventName,
            PersonId = evenDbModel.Person.Id!,
            StartDate = evenDbModel.StartDate.ToDateOnly(),
            EndDate = evenDbModel.EndDate.ToDateOnly(),
            StartTime = evenDbModel.StartTime.ToTimeOnly(),
            EndTime = evenDbModel.EndTime.ToTimeOnly(),
            RelevantForDinner = evenDbModel.RelevantForDinner,
            DinnerAt = evenDbModel.DinnerAt?.ToTimeOnly()
        };
    }
}