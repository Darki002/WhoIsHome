using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.External.Database;
using WhoIsHome.External.PushUp;
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
    public async Task<IActionResult> Post([FromBody] PushUpSettingsDto pushUpSettings, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var settings = await context.PushUpSettings.SingleOrDefaultAsync(s => s.UserId == userContext.UserId, cancellationToken);

        if (settings is null)
        {
            await CreateSettingAsync(pushUpSettings, cancellationToken);
            return Ok("ExpoPushToken is saved.");
        }

        await UpdateSettingAsync(pushUpSettings, settings, cancellationToken);
        return Ok("ExpoPushToken is saved.");
    }

    private async Task CreateSettingAsync(PushUpSettingsDto pushUpSettings, CancellationToken cancellationToken)
    {
        var settings = new PushUpSettings
        {
            Enabled = pushUpSettings.Enable ?? true,
            UserId = userContext.UserId,
            Token = pushUpSettings.Token,
            LanguageCode = Convert(pushUpSettings.LanguageCode, CultureInfo.GetCultureInfo("en"))
        };
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        await context.PushUpSettings.AddAsync(settings, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateSettingAsync(PushUpSettingsDto pushUpSettings, PushUpSettings settings, CancellationToken cancellationToken)
    {
        settings.Token = pushUpSettings.Token;
        settings.Enabled = pushUpSettings.Enable ?? settings.Enabled;
        settings.LanguageCode = Convert(pushUpSettings.LanguageCode, settings.LanguageCode!);
        
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        context.PushUpSettings.Update(settings);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static CultureInfo Convert(string? languageCode, CultureInfo fallback)
    {
        return languageCode is not null
            ? CultureInfo.GetCultureInfo(languageCode)
            : fallback;
    }
}