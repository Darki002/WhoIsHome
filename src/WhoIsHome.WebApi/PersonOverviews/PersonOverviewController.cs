using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.PersonOverview;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.PersonOverviews;

public class PersonOverviewController(PersonOverviewQueryHandler queryHandler, IUserContext userContext) : WhoIsHomeControllerBase<PersonOverview, UserOverviewModel>
{
    [HttpGet]
    public async Task<ActionResult<UserOverviewModel>> GetCurrentPersonOverviewAsync(CancellationToken cancellationToken)
    {
        return await GetPersonOverviewAsync(userContext.UserId, cancellationToken);
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserOverviewModel>> GetPersonOverviewAsync(int userId, CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(userId, cancellationToken);
        return await BuildResponseAsync(result);
    }
    
    protected override Task<UserOverviewModel> ConvertToModelAsync(PersonOverview data)
    {
        var model = new UserOverviewModel
        {
            UserId = data.User.Id!.Value,
            Today = data.Today.Select(PersonOverviewEventModel.From).ToList(),
            ThisWeek = data.ThisWeek.Select(PersonOverviewEventModel.From).ToList(),
            FutureEvents = data.FutureEvents.Select(PersonOverviewEventModel.From).ToList()
        };

        return Task.FromResult(model);
    }
}