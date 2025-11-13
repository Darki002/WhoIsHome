using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Test.TestData;

public static class EventGroupTestData
{
    public static EventGroup CreateDefaultWithDefaultDateTimes(
        int id = 1,
        string title = "Test",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        WeekDay weekDays = WeekDay.Monday,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        PresenceType presenceType = PresenceType.Default,
        TimeOnly? dinnerTime = null!,
        int userId = 1)
    {
        startDate ??= new DateOnly(2024, 10, 10);
        endDate ??= new DateOnly(2024, 11, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);
        dinnerTime ??= new TimeOnly(18, 00, 00);

        return new EventGroup(
            title: title,
            startDate: startDate.Value,
            endDate: endDate.Value,
            weekDays: weekDays,
            startTime: startTime.Value,
            endTime: endTime.Value,
            presenceType: presenceType,
            dinnerTime: dinnerTime,
            userId) { Id = id };
    }
    
    public static EventGroup CreateDefault(
        int id = 1,
        string title = "Test",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        WeekDay weekDays = WeekDay.Monday,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        PresenceType presenceType = PresenceType.Default,
        TimeOnly? dinnerTime = null!,
        int userId = 1)
    {
        startDate ??= new DateOnly(2024, 10, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        dinnerTime ??= new TimeOnly(18, 00, 00);

        return new EventGroup(
            title: title, 
            startDate: startDate.Value, 
            endDate: endDate, 
            weekDays: weekDays,
            startTime: startTime.Value, 
            endTime: endTime,
            presenceType: presenceType,
            dinnerTime: dinnerTime,
            userId) { Id = id };
    }
    
    public static EventGroup NotPresentWithDefaultDateTimes(
        string title = "Test",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        WeekDay weekDays = WeekDay.Monday,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        int userId = 1)
    {
        startDate ??= new DateOnly(2024, 10, 10);
        endDate ??= new DateOnly(2024, 11, 10);
        startTime ??= new TimeOnly(16, 00, 00);
        endTime ??= new TimeOnly(17, 00, 00);

        return new EventGroup(
            title: title, 
            startDate: startDate.Value, 
            endDate: endDate.Value, 
            weekDays: weekDays,
            startTime: startTime.Value, 
            endTime: endTime.Value,
            presenceType: PresenceType.NotPresent,
            dinnerTime: null,
            userId);
    }
    
    public static EventGroup NotPresent(
        string title = "Test",
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        WeekDay weekDays = WeekDay.Monday,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null,
        int userId = 1)
    {
        startDate ??= new DateOnly(2024, 10, 10);
        startTime ??= new TimeOnly(16, 00, 00);

        return new EventGroup(
            title: title, 
            startDate: startDate.Value, 
            endDate: endDate, 
            weekDays: weekDays,
            startTime: startTime.Value, 
            endTime: endTime,
            presenceType: PresenceType.NotPresent,
            dinnerTime: null,
            userId);
    }
}