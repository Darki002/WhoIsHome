using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Test.TestData;

public static class RepeatedEventTestData
{
    public static RepeatedEvent CreateDefault(
        string title = "Test",
        DateOnly? firstOccurrence = null,
        DateOnly? lastOccurrence = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        PresenceType presenceType = PresenceType.Default,
        TimeOnly? dinnerTime = null!,
        int userId = 1)
    {
        firstOccurrence ??= new DateOnly(2024, 10, 10);
        lastOccurrence ??= new DateOnly(2024, 11, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);
        dinnerTime ??= new TimeOnly(18, 00, 00);

        return new RepeatedEvent(
            id: null, 
            title: title, 
            firstOccurrence: firstOccurrence.Value, 
            lastOccurrence: lastOccurrence.Value, 
            startTime: startTime.Value, 
            endTime: endTime.Value,
            DinnerTime.Create(presenceType, dinnerTime),
            userId);
    }
    
    public static RepeatedEvent NotPresent(
        string title = "Test",
        DateOnly? firstOccurrence = null,
        DateOnly? lastOccurrence = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        int userId = 1)
    {
        firstOccurrence ??= new DateOnly(2024, 10, 10);
        lastOccurrence ??= new DateOnly(2024, 11, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);

        return new RepeatedEvent(
            id: null, 
            title: title, 
            firstOccurrence: firstOccurrence.Value, 
            lastOccurrence: lastOccurrence.Value, 
            startTime: startTime.Value, 
            endTime: endTime.Value,
            DinnerTime.Create(PresenceType.NotPresent, null),
            userId);
    }
}