using WhoIsHome.Aggregates;

namespace WhoIsHome.QueryHandler.PersonOverview;

public enum EventType
{
    OneTimeEvent,
    RepeatedEvent
}

public static class EventTypeHelper
{
    public static EventType FromType(EventBase eventBase)
    {
        return eventBase switch
        {
            OneTimeEvent => EventType.OneTimeEvent,
            RepeatedEvent => EventType.RepeatedEvent,
            _ => throw new InvalidOperationException(
                $"There is no {nameof(EventType)} for type of {eventBase.GetType().Name}")
        };
    }
}