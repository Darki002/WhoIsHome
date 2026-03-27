using WhoIsHome.Entities;

namespace WhoIsHome.Handlers;

public interface IEventUpdateHandler
{
    Task HandleAsync(int userId, IEnumerable<EventInstance> updatedEvents, EventUpdateHandler.UpdateAction updateAction);
}