using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.WeeklyReports;

namespace WhoIsHome.WebApi.QueryServices.WeeklyReports;

[Authorize]
[Route("api/v1/quarries/weekly-report")]
public class WeeklyReportController(WeeklyReportQueryHandler queryHandler) : Controller
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<WeeklyReportModel>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WeeklyReportModel>> GetAsync(CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(cancellationToken);
        
        var errors = result.Where(r => r.Report.ErrorMessage is not null).ToList();
        if (errors.Count > 0)
        {
            return BadRequest(errors.Select(e => e.Report.ErrorMessage));
        }
        
        return Ok(ToModel(result));
    }

    private static List<WeeklyReportModel> ToModel(IReadOnlyCollection<WeeklyReport> data)
    {
        return data
            .Select(WeeklyReportModel.From)
            .ToList();
    }
}
