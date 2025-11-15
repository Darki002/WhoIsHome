using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.QueryHandler.UserOverview;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.QueryServices.UserOverviews;

[Authorize] 

public class UserOverviewController(UserOverviewQueryHandler queryHandler, IUserContext userContext) : Controller
{
    [HttpGet]
    [ProducesResponseType<UserOverviewModel>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUserOverviewAsync(CancellationToken cancellationToken)
    {
        return await GetUserOverviewAsync(userContext.UserId, cancellationToken);
    }

    [HttpGet("{userId:int}")]
    [ProducesResponseType<UserOverviewModel>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserOverviewAsync(int userId, CancellationToken cancellationToken)
    {
        var result = await queryHandler.HandleAsync(userId, cancellationToken);
        return Ok(ToModel(result));
    }

    private static UserOverviewModel ToModel(UserOverview data)
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