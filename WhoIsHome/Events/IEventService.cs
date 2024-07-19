using Galaxus.Functional;

namespace WhoIsHome.Events;

public interface IEventService
{
    Task<Result<Event, string>> GetAsync(string id);
}