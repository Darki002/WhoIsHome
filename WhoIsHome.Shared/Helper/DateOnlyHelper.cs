namespace WhoIsHome.Shared.Helper;

public static class DateOnlyHelper
{
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    
    public static bool IsThisWeek(this DateOnly dateOnly)
    {
        return dateOnly.IsSameWeek(DateTime.Today);
    }

    public static bool IsSameWeek(this DateOnly dateOnly, DateTime dateTime)
    {
        var date1 = dateOnly.ToDateTime(TimeOnly.MinValue);
        var date2 = dateTime.Date;
        
        var d1 = date1.StartOfWeek();
        var d2 = date2.StartOfWeek();

        return d1 == d2;
    }
    
    public static bool IsSameWeek(this DateOnly dateOnly, DateOnly dateOnly2)
    {
        var dateTime = dateOnly2.ToDateTime(TimeOnly.MinValue);
        return dateOnly.IsSameWeek(dateTime);
    }
    
    public static DateTime StartOfWeek(this DateTime dt)
    {
        var diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static int DaysUntilNext(this DateTime date, DayOfWeek targetDay)
    {
        var difference = targetDay.Normalize() - date.DayOfWeek.Normalize();
        return difference < 0 ? difference + 7 : difference;
    }

    private static DayOfWeek Normalize(this DayOfWeek dayOfWeek)
    {
        return (DayOfWeek)(((int)dayOfWeek + 6) % 7);
    }
}