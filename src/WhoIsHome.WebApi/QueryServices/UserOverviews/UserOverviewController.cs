using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.QueryServices.UserOverviews;

[Authorize]
public class UserOverviewController(UserOverviewQueryHandler queryHandler, IUserContext userContext) : Controller
{
    [HttpGet]
    public async Task<ActionResult<UserOverviewModel>> GetCurrentUserOverviewAsync(CancellationToken cancellationToken)
    {
        return await GetUserOverviewAsync(userContext.UserId, cancellationToken);
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserOverviewModel>> GetUserOverviewAsync(int userId, CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(userId, cancellationToken);
        return Ok(ToModel(result));
    }

    private static UserOverviewModel ToModel(UserOverviewMock data)
    {
        return new UserOverviewModel
        {
            UserId = data.User.Id,
            Today = data.Today.Select(UserOverviewEventModel.From).ToList(),
            ThisWeek = data.ThisWeek.Select(UserOverviewEventModel.From).ToList(),
            FutureEvents = data.FutureEvents.Select(UserOverviewEventModel.From).ToList()
        };
    }
}