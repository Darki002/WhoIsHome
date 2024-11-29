using WhoIsHome.Shared.Helper;

namespace WhoIsHome.Test;

public class DateTimeProviderFake : IDateTimeProvider
{
    public DateTime Now { get; set; } = new DateTime(2024, 11, 26, 19, 00, 00);

    public TimeOnly CurrentTime { get; set; } = new TimeOnly(19, 00);
    
    public DateOnly CurrentDate { get; set; } = new DateOnly(2024, 11, 26);
}