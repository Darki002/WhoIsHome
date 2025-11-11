using WhoIsHome.Entities;

namespace WhoIsHome.QueryHandler.WeeklyReports;

public record WeeklyReportMock
{
    public required User User { get; init; }

    public required WeeklyReportResult Report { get; init; }
}