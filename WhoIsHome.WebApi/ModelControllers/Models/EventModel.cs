using WhoIsHome.Services.Events;

namespace WhoIsHome.WebApi.ModelControllers.Models;

public class EventModel
{
    public string Id { get; set; } = null!;

    public string EventName { get; set; } = null!;

    public PersonModel Person { get; set; } = null!;
    
    public DateTime Date { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public bool RelevantForDinner { get; set; }
    
    public DateTime? DinnerAt { get; set; }
    
    public static EventModel From(Event evenDbModel)
    {
        return new EventModel
        {
            Id = evenDbModel.Id!,
            EventName = evenDbModel.EventName,
            Person = PersonModel.From(evenDbModel.Person),
            Date = evenDbModel.Date.ToDateTime(),
            StartTime = evenDbModel.StartTime.ToDateTime(),
            EndTime = evenDbModel.EndTime.ToDateTime(),
            RelevantForDinner = evenDbModel.RelevantForDinner,
            DinnerAt = evenDbModel.DinnerAt?.ToDateTime()
        };
    }
}