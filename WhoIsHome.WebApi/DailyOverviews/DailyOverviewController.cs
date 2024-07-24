using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.WebApi.ModelControllers.Models;

namespace WhoIsHome.WebApi.DailyOverviews;

public class DailyOverviewController(DailyOverviewQueryHandler queryHandler) 
    : WhoIsHomeControllerBase<IReadOnlyCollection<PersonPresence>, IReadOnlyCollection<DailyOverview>>
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DailyOverview>>> GetAsync(CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(cancellationToken);
        return BuildResponse(result);
    }

    protected override IReadOnlyCollection<DailyOverview> ConvertToModel(IReadOnlyCollection<PersonPresence> data)
    {
        return data
            .Select(presence => 
                new DailyOverview
                {
                    Person = PersonModel.From(presence.Person), 
                    IsAtHome = presence.IsAtHome, 
                    DinnerAt = presence.DinnerAt?.ToString()
                })
            .ToList();
    }
}