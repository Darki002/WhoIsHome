using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Events;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EventController(IEventService eventService) : WhiIsHomeControllerBase<Event, EventModel>
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(string id, CancellationToken cancellationToken)
    {
        var result = await eventService.GetAsync(id, cancellationToken);
        return BuildResponse(result, EventModel.From);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] NewEventModel eventModel, CancellationToken cancellationToken)
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
        
        return BuildResponse(result, EventModel.From);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateEvent([FromBody] EventModel eventModel, CancellationToken cancellationToken)
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
        
        return BuildResponse(result, EventModel.From);
    }
}