using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.WebApi.QueryServices.DailyOverviews;

public class DailyOverviewController(DailyOverviewQueryHandler queryHandler, IDateTimeProvider dateTimeProvider) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DailyOverviewModel>>> GetAsync(
        CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(dateTimeProvider.CurrentDate, cancellationToken);

        var errors = result.Where(r => r.HasError).ToList();
        if (errors.Count > 0)
        {
            return BadRequest(errors.Select(e => e.ErrorMessage));
        }
        
        return Ok(ToModel(result));
    }

    private static List<DailyOverviewModel> ToModel(IReadOnlyCollection<DailyOverview> data)
    {
        return data
            .Select(DailyOverviewModel.From)
            .ToList();
    }
}