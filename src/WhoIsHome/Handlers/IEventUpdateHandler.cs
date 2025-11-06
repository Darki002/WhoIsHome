using WhoIsHome.External.Models;

namespace WhoIsHome.Handlers;

public interface IEventUpdateHandler
{
    Task HandleAsync(EventModel updatedEvent, EventUpdateHandler.UpdateAction updateAction);
}