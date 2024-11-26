using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Test;

public class DateTimeProviderFake : IDateTimeProvider
{
    public DateTime Now { get; } = new DateTime(2024, 11, 26, 19, 00, 00);

    public TimeOnly CurrentTime { get; } = new TimeOnly(19, 00);
    
    public DateOnly CurrentDate { get; } = new DateOnly(2024, 11, 26);
}