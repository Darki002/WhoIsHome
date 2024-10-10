namespace WhoIsHome.Shared.Helper;

public static class DateOnlyHelper
{
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    
    public static bool IsThisWeek(this DateOnly dateOnly1)
    {
        return IsThisWeek(dateOnly1, DateTime.Today);
    }

    internal static bool IsThisWeek(DateOnly dateOnly, DateTime dateTime)
    {
        var date1 = dateOnly.ToDateTime(TimeOnly.MinValue);
        var date2 = dateTime.Date;
        
        var d1 = date1.StartOfWeek();
        var d2 = date2.StartOfWeek();

        return d1 == d2;
    }
    
    public static DateTime StartOfWeek(this DateTime dt)
    {
        var diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}