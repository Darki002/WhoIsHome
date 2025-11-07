using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
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
            LanguageCode = Convert(pushUpSettings.LanguageCode, CultureInfo.GetCultureInfo("en"))
        };
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.PushUpSettings.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateSettingAsync(PushUpSettings pushUpSettings, PushUpSettingsModel model, CancellationToken cancellationToken)
    {
        model.Token = pushUpSettings.Token;
        model.Enabled = pushUpSettings.Enable ?? model.Enabled;
        model.LanguageCode = Convert(pushUpSettings.LanguageCode, model.LanguageCode!);
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.PushUpSettings.Update(model);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static CultureInfo Convert(string? languageCode, CultureInfo fallback)
    {
        return languageCode is not null
            ? CultureInfo.GetCultureInfo(languageCode)
            : fallback;
    }
}