using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Events;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EventController(IEventService eventService) : WhiIsHomeControllerBase<Event, EventModel>
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(string id)
    {
        var result = await eventService.GetAsync(id);
        return BuildResponse(result, EventModel.From);
    }
}