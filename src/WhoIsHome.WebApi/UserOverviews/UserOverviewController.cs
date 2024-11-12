using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.UserOverviews;

public class UserOverviewController(UserOverviewQueryHandler queryHandler, IUserContext userContext) : WhoIsHomeControllerBase<UserOverview, UserOverviewModel>
{
    [HttpGet]
    public async Task<ActionResult<UserOverviewModel>> GetCurrentUserOverviewAsync(CancellationToken cancellationToken)
    {
        return await GetUserOverviewAsync(userContext.UserId, cancellationToken);
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserOverviewModel>> GetUserOverviewAsync(int userId, CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(userId, cancellationToken);
        return await BuildResponseAsync(result);
    }
    
    protected override Task<UserOverviewModel> ConvertToModelAsync(UserOverview data)
    {
        var model = new UserOverviewModel
        {
            UserId = data.User.Id!.Value,
            Today = data.Today.Select(UserOverviewEventModel.From).ToList(),
            ThisWeek = data.ThisWeek.Select(UserOverviewEventModel.From).ToList(),
            FutureEvents = data.FutureEvents.Select(UserOverviewEventModel.From).ToList()
        };

        return Task.FromResult(model);
    }
}