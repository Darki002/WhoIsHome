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
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var model = await context.PushUpSettings.SingleOrDefaultAsync(s => s.UserId == userContext.UserId, cancellationToken);

        model ??= new PushUpSettingsModel
        {
            Enabled = pushUpSettings.Enable ?? true,
            UserId = userContext.UserId
        };

        model.Enabled = pushUpSettings.Enable ?? model.Enabled;
        model.Token = pushUpSettings.Token;
         
        await context.PushUpSettings.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Ok("ExpoPushToken is saved.");
    }
}