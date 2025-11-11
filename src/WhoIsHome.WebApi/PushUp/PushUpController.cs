using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.External.Database;
using WhoIsHome.External.PushUp;
using WhoIsHome.Shared.Authentication;

namespace WhoIsHome.WebApi.PushUp;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PushUpController(
    IUserContext userContext,
    WhoIsHomeContext context,
    ILogger<PushUpController> logger) 
    : Controller
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PushUpSettingsDto pushUpSettings, CancellationToken cancellationToken)
    {
        var language = Convert(pushUpSettings.LanguageCode, CultureInfo.GetCultureInfo("en"));
        if (language is null)
        {
            return BadRequest(new  { Message = $"Unknown Language Code {pushUpSettings.LanguageCode}." });
        }
        
        var settings = await context.PushUpSettings.SingleOrDefaultAsync(s => s.UserId == userContext.UserId, cancellationToken);

        if (settings is not null)
        {
            settings.Token = pushUpSettings.Token;
            settings.Enabled = pushUpSettings.Enable ?? settings.Enabled;
            settings.LanguageCode = Convert(pushUpSettings.LanguageCode, settings.LanguageCode!);
            context.PushUpSettings.Update(settings);
        }
        else
        {
            var newSettings = new PushUpSettings
            {
                Enabled = pushUpSettings.Enable ?? true,
                UserId = userContext.UserId,
                Token = pushUpSettings.Token,
                LanguageCode = Convert(pushUpSettings.LanguageCode, CultureInfo.GetCultureInfo("en"))
            };
            await context.PushUpSettings.AddAsync(newSettings, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Ok("ExpoPushToken is saved.");
    }

    private CultureInfo? Convert(string? languageCode, CultureInfo fallback)
    {
        try
        {
            return languageCode is not null
                ? CultureInfo.GetCultureInfo(languageCode)
                : fallback;
        }
        catch
        {
            logger.LogError("Failed to create Culture from {Orig}.", languageCode);
            return null;
        }
    }
}