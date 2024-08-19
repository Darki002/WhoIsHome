using WhoIsHome.Services.Events;

namespace WhoIsHome.WebApi.ModelControllers.Models;

public class EventModel
{
    public string Id { get; set; } = null!;

    public string EventName { get; set; } = null!;

    public PersonModel Person { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool RelevantForDinner { get; set; }

    public TimeOnly? DinnerAt { get; set; }

    public static EventModel From(Event evenDbModel)
    {
        return new EventModel
        {
            Id = evenDbModel.Id!,
            EventName = evenDbModel.EventName,
            Person = PersonModel.From(evenDbModel.Person),
            Date = DateOnly.FromDateTime(evenDbModel.Date.ToDateTime()),
            StartTime = TimeOnly.FromDateTime(evenDbModel.StartTime.ToDateTime()),
            EndTime = TimeOnly.FromDateTime(evenDbModel.EndTime.ToDateTime()),
            RelevantForDinner = evenDbModel.RelevantForDinner,
            DinnerAt = evenDbModel.DinnerAt.HasValue
                ? TimeOnly.FromDateTime(evenDbModel.DinnerAt!.Value.ToDateTime())
                : null
        };
    }
}