using WhoIsHome.Entities;

namespace WhoIsHome.QueryHandler.WeeklyReports;

public record WeeklyReport
{
    public required User User { get; init; }

    public required WeeklyReportResult Report { get; init; }
}