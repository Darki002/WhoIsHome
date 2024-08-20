using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.WebApi.ModelControllers.Models;

public class RepeatedEventModel
{
    public string Id { get; set; } = null!;

    public string EventName { get; set; } = null!;

    public PersonModel Person { get; set; } = null!;
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public TimeOnly StartTime { get; set; }
    
    public TimeOnly EndTime { get; set; }
    
    public bool RelevantForDinner { get; set; }
    
    public TimeOnly? DinnerAt { get; set; }
    
    public static RepeatedEventModel From(RepeatedEvent evenDbModel)
    {
        return new RepeatedEventModel
        {
            Id = evenDbModel.Id!,
            EventName = evenDbModel.EventName,
            Person = PersonModel.From(evenDbModel.Person),
            StartDate = evenDbModel.StartDate.ToDateOnly(),
            EndDate = evenDbModel.EndDate.ToDateOnly(),
            StartTime = evenDbModel.StartTime.ToTimeOnly(),
            EndTime = evenDbModel.EndTime.ToTimeOnly(),
            RelevantForDinner = evenDbModel.RelevantForDinner,
            DinnerAt = evenDbModel.DinnerAt?.ToTimeOnly()
        };
    }
}