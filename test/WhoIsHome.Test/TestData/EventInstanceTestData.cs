using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Test.TestData;

public static class EventInstanceTestData
{
    public static EventInstance CreateDefault(
        int id = 1,
        string title = "Test",
        DateOnly? date = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        PresenceType presenceType = PresenceType.Default,
        TimeOnly? dinnerTime = null,
        bool isOriginal = true,
        DateOnly? originalDate = null,
        int eventGroupId = 1,
        int userId = 1)
    {
        date ??= new DateOnly(2024, 10, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);
        dinnerTime ??= new TimeOnly(18, 00, 00);

        return new EventInstance
        {
            Id = id,
            Title = title,
            Date = date.Value,
            StartTime = startTime.Value,
            EndTime = endTime,
            PresenceType = presenceType,
            DinnerTime = dinnerTime,
            IsOriginal = isOriginal,
            OriginalDate = originalDate ?? date.Value,
            EventGroupId = eventGroupId,
            UserId = userId
        };
    }
    
    public static EventInstance CreateWithNotPresent(
        string title = "Test",
        DateOnly? date = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        int eventGroupId = 1,
        int userId = 1)
    {
        date ??= new DateOnly(2024, 10, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);

        return new EventInstance
        {
            Id = 0,
            Title = title,
            Date = date.Value,
            StartTime = startTime.Value,
            EndTime = endTime,
            PresenceType = PresenceType.NotPresent,
            DinnerTime = null,
            IsOriginal = true,
            OriginalDate = date.Value,
            EventGroupId = eventGroupId,
            UserId = userId
        };
    }
}