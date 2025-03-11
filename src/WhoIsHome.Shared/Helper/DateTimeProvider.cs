namespace WhoIsHome.Shared.Helper;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.UtcNow;
    
    public TimeOnly CurrentTime => TimeOnly.FromDateTime(DateTime.UtcNow);
    
    public DateOnly CurrentDate => DateOnly.FromDateTime(DateTime.UtcNow);
}