using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.WeeklyReports;

namespace WhoIsHome.WebApi.QueryServices.WeeklyReports;

public class WeeklyReportController(WeeklyReportQueryHandler queryHandler) 
    : WhoIsHomeControllerBase<IReadOnlyCollection<WeeklyReport>, IReadOnlyCollection<WeeklyReportModel>>
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<WeeklyReportModel>>> GetAsync(
        CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(cancellationToken);
        return await BuildResponseAsync(result);
    }

    protected override Task<IReadOnlyCollection<WeeklyReportModel>> ConvertToModelAsync(IReadOnlyCollection<WeeklyReport> data)
    {
        return Task.FromResult<IReadOnlyCollection<WeeklyReportModel>>(data
            .Select(WeeklyReportModel.From)
            .ToList());
    }
}