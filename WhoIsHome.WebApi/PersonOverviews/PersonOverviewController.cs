using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.PersonOverview;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.PersonOverviews;

public class PersonOverviewController(PersonOverviewQueryHandler queryHandler) : WhoIsHomeControllerBase<PersonOverview, UserOverviewModel>
{
    [HttpGet("/{personId}")]
    public async Task<ActionResult<UserOverviewModel>> GetPersonOverviewAsync(int personId, CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(personId, cancellationToken);
        return BuildResponse(result);
    }
    
    protected override UserOverviewModel ConvertToModel(PersonOverview data)
    {
        return new UserOverviewModel
        {
            User = UserModel.From(data.User),
            Today = data.Today.Select(PersonOverviewEventModel.From).ToList(),
            ThisWeek = data.ThisWeek.Select(PersonOverviewEventModel.From).ToList(),
            FutureEvents = data.FutureEvents.Select(PersonOverviewEventModel.From).ToList()
        };
    }
}