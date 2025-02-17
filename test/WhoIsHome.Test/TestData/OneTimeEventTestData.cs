using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Test.TestData;

public static class OneTimeEventTestData
{
    public static OneTimeEvent CreateDefault(
        string title = "Test",
        DateOnly? date = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        PresenceType presenceType = PresenceType.Default,
        TimeOnly? dinnerTime = null,
        int userId = 1)
    {
        date ??= new DateOnly(2024, 10, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);
        dinnerTime ??= new TimeOnly(18, 00, 00);

        return new OneTimeEvent(
            id: null, 
            title: title, 
            date: date.Value, 
            startTime: startTime.Value, 
            endTime: endTime.Value,
            DinnerTime.Create(presenceType, dinnerTime),
            userId);
    }
    
    public static OneTimeEvent CreateWithNotPresent(
        string title = "Test",
        DateOnly? date = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        int userId = 1)
    {
        date ??= new DateOnly(2024, 10, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);

        return new OneTimeEvent(
            id: null, 
            title: title, 
            date: date.Value, 
            startTime: startTime.Value, 
            endTime: endTime.Value,
            DinnerTime.Create(PresenceType.NotPresent, null),
            userId);
    }
}