using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services.Events;
using WhoIsHome.WebApi.ModelControllers.Models;

namespace WhoIsHome.WebApi.ModelControllers;

public class EventController(IEventService eventService) : ModelControllerBase<Event, EventModel>(eventService)
{
    [HttpPost]
    public async Task<ActionResult<EventModel>> CreateEvent([FromBody] NewEventModel eventModel, CancellationToken cancellationToken)
    {
        var result = await eventService.CreateAsync(
            eventName: eventModel.EventName,
            personId: eventModel.PersonId,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            relevantForDinner: eventModel.RelevantForDinner,
            dinnerAt: eventModel.DinnerAt,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    [HttpPut]
    public async Task<ActionResult<EventModel>> UpdateEvent([FromBody] EventModel eventModel, CancellationToken cancellationToken)
    {
        var result = await eventService.UpdateAsync(
            id: eventModel.Id,
            eventName: eventModel.EventName,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            relevantForDinner: eventModel.RelevantForDinner,
            dinnerAt: eventModel.DinnerAt,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    [HttpGet("list/{personId}")]
    public async Task<ActionResult<IReadOnlyList<EventModel>>> GetByPerson(string personId, CancellationToken cancellationToken)
    {
        var result = await eventService.GetByPersonIdAsync(personId, cancellationToken);
        return BuildResponse(result);
    }

    protected override EventModel ConvertToModel(Event data) => EventModel.From(data);
}