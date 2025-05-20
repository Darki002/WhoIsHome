using System.Globalization;
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

        if (model is null)
        {
            await CreateSettingAsync(pushUpSettings, cancellationToken);
            return Ok("ExpoPushToken is saved.");
        }

        await UpdateSettingAsync(pushUpSettings, model, cancellationToken);
        return Ok("ExpoPushToken is saved.");
    }

    private async Task CreateSettingAsync(PushUpSettings pushUpSettings, CancellationToken cancellationToken)
    {
        var model = new PushUpSettingsModel
        {
            Enabled = pushUpSettings.Enable ?? true,
            UserId = userContext.UserId,
            Token = pushUpSettings.Token,
            LanguageCode = pushUpSettings.LanguageCode ?? CultureInfo.GetCultureInfo("en")
        };
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.PushUpSettings.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateSettingAsync(PushUpSettings pushUpSettings, PushUpSettingsModel model, CancellationToken cancellationToken)
    {
        model.Token = pushUpSettings.Token;
        model.Enabled = pushUpSettings.Enable ?? model.Enabled;
        model.LanguageCode = pushUpSettings.LanguageCode ?? model.LanguageCode;
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.PushUpSettings.Update(model);
        await context.SaveChangesAsync(cancellationToken);
    }
}