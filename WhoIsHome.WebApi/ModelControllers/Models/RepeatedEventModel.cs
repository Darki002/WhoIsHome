using WhoIsHome.Services.RepeatedEvents;

namespace WhoIsHome.WebApi.ModelControllers.Models;

public class RepeatedEventModel
{
    public string Id { get; set; } = null!;

    public string EventName { get; set; } = null!;

    public PersonModel Person { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public bool RelevantForDinner { get; set; }
    
    public DateTime? DinnerAt { get; set; }
    
    public static RepeatedEventModel From(RepeatedEvent evenDbModel)
    {
        return new RepeatedEventModel
        {
            Id = evenDbModel.Id!,
            EventName = evenDbModel.EventName,
            Person = PersonModel.From(evenDbModel.Person),
            StartDate = evenDbModel.StartDate.ToDateTime(),
            EndDate = evenDbModel.EndDate.ToDateTime(),
            StartTime = evenDbModel.StartTime.ToDateTime(),
            EndTime = evenDbModel.EndTime.ToDateTime(),
            RelevantForDinner = evenDbModel.RelevantForDinner,
            DinnerAt = evenDbModel.DinnerAt?.ToDateTime()
        };
    }
}