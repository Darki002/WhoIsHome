using WhoIsHome.Entities;

namespace WhoIsHome.Handlers;

public interface IEventUpdateHandler
{
    Task HandleAsync(EventInstance updatedEvent, EventUpdateHandler.UpdateAction updateAction);
}