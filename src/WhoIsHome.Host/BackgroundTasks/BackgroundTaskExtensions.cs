namespace WhoIsHome.Host.BackgroundTasks;

public static class BackgroundTaskHelpers
{
    public static DateTime CalculateNextRun(DateTime from, DayOfWeek day, TimeSpan at)
    {
        var current = (int)from.DayOfWeek;
        var target  = (int)day;
        var daysUntil = (target - current + 7) % 7;
        if (daysUntil == 0 && from.TimeOfDay >= at) daysUntil = 7;

        return from.Date.AddDays(daysUntil).Add(at);
    }
}