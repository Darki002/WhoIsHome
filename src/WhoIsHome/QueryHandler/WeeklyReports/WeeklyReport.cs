using WhoIsHome.Entities;

namespace WhoIsHome.QueryHandler.WeeklyReports;

public record WeeklyReport
{
    public required User User { get; init; }

    public required Dictionary<DateOnly, (bool IsAtHome, TimeOnly? DinnerTime)> DailyOverviews { get; init; }
}