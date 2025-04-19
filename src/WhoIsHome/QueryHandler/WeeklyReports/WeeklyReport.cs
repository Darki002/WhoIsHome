namespace WhoIsHome.QueryHandler.WeeklyReports;

public record WeeklyReport
{
    public required int UserId { get; init; }

    public required Dictionary<DateOnly, (bool IsAtHome, TimeOnly? DinnerTime)> DailyOverviews { get; init; }
}