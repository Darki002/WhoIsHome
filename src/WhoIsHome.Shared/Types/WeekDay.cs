namespace WhoIsHome.Shared.Types;

[Flags]
public enum WeekDay
{
    Sunday = 1 << 0,
    Monday = 1 << 1,
    Tuesday = 1 << 2,
    Wednesday = 1 << 3,
    Thursday = 1 << 4,
    Friday = 1 << 5,
    Saturday = 1 << 6,
}

public static class WeekDayExtensions
{
    public static DayOfWeek ToDayOfWeek(this WeekDay weekDay)
    {
        return weekDay switch
        {
            WeekDay.Sunday => DayOfWeek.Sunday,
            WeekDay.Monday => DayOfWeek.Monday,
            WeekDay.Tuesday => DayOfWeek.Tuesday,
            WeekDay.Wednesday => DayOfWeek.Wednesday,
            WeekDay.Thursday => DayOfWeek.Thursday,
            WeekDay.Friday => DayOfWeek.Friday,
            WeekDay.Saturday => DayOfWeek.Saturday,
            _ => throw new ArgumentOutOfRangeException(nameof(weekDay), $"Invalid WeekDay value: {weekDay}")
        };
    }

    public static WeekDay ToWeekDay(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => WeekDay.Sunday,
            DayOfWeek.Monday => WeekDay.Monday,
            DayOfWeek.Tuesday => WeekDay.Tuesday,
            DayOfWeek.Wednesday => WeekDay.Wednesday,
            DayOfWeek.Thursday => WeekDay.Thursday,
            DayOfWeek.Friday => WeekDay.Friday,
            DayOfWeek.Saturday => WeekDay.Saturday,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), $"Invalid DayOfWeek value: {dayOfWeek}")
        };
    }
    
    public static List<DayOfWeek> ToDayOfWeekList(this WeekDay weekDays)
    {
        var result = new List<DayOfWeek>();

        foreach (WeekDay day in Enum.GetValues(typeof(WeekDay)))
        {
            if (weekDays.HasFlag(day))
                result.Add(day.ToDayOfWeek());
        }

        return result;
    }
    
    public static WeekDay ToWeekDays(this IEnumerable<DayOfWeek> weekDays)
    {
        WeekDay result = 0;
        
        foreach (var day in weekDays)
        {
            result |= day.ToWeekDay();
        }

        return result;
    }
}