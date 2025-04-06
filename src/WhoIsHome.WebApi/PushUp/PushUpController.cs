using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.External.Models;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.PushUp;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PushUpController(
    IUserContext userContext,
    IDbContextFactory<WhoIsHomeContext> contextFactory) 
    : Controller
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PushUpSettings pushUpSettings, CancellationToken cancellationToken)
    {
        if (pushUpSettings.Token is null)
        {
            return BadRequest("ExpoPushToken is required.");
        }

        var model = new PushUpSettingsModel
        {
            Token = pushUpSettings.Token,
            Enabled = pushUpSettings.Enable ?? false,
            UserId = userContext.UserId
        };
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.PushUpSettings.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Ok("ExpoPushToken is saved.");
    }
}