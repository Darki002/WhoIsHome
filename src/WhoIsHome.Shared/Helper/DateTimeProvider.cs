namespace WhoIsHome.Shared.Helper;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get; } = DateTime.UtcNow;
    
    public TimeOnly CurrentTime { get; } = TimeOnly.FromDateTime(DateTime.UtcNow);
    
    public DateOnly CurrentDate { get; } = DateOnly.FromDateTime(DateTime.UtcNow);
}