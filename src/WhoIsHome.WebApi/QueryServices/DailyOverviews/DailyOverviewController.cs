using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.DailyOverview;

namespace WhoIsHome.WebApi.QueryServices.DailyOverviews;

public class DailyOverviewController(DailyOverviewQueryHandler queryHandler)
    : WhoIsHomeControllerBase<IReadOnlyCollection<DailyOverview>, IReadOnlyCollection<DailyOverviewModel>>
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DailyOverviewModel>>> GetAsync(
        CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(cancellationToken);
        return await BuildResponseAsync(result);
    }

    protected override Task<IReadOnlyCollection<DailyOverviewModel>> ConvertToModelAsync(
        IReadOnlyCollection<DailyOverview> data)
    {
        var result = data
            .Select(DailyOverviewModel.From)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<DailyOverviewModel>>(result);
    }
}