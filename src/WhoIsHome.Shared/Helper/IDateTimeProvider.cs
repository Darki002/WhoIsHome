namespace WhoIsHome.Shared.Helper;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
    
    public TimeOnly CurrentTime { get; }
    
    public DateOnly CurrentDate { get; }
}