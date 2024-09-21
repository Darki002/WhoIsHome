namespace WhoIsHome.Shared;

public static class DateOnlyHelper
{
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

    public static int WeekOfYear(this DateOnly dateOnly) => dateOnly.DayOfYear / 7;
    
    public static bool IsThisWeek(this DateOnly dateOnly1)
    {
        var date1 = dateOnly1.ToDateTime(TimeOnly.MinValue);
        var date2 = DateTime.Today;
        
        var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
        var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
        var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

        return d1 == d2;
    }
}