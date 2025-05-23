using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhoIsHome.External.PushUp.ApiClient;
using WhoIsHome.External.Translation;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.External.PushUp;

public class PushUpContext(
    PushApiClient client, 
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    IConfiguration configuration,
    ITranslationService translation,
    ILogger<PushApiClient> logger) 
    : IPushUpContext
{
    public async Task PushEventUpdateAsync(PushUpCommand command)
    {
        if (configuration.GetPushNotificationEnabled() is false)
        {
            logger.LogInformation("PushUp is disabled! Won't send any Push Notifications. To enable it, set 'PUSH_UP_ENABLED' to true in the environment Variables.");
            return;
        }
        
        await SendAsync(command);
    }

    private async Task SendAsync(PushUpCommand command)
    {
        var translationGroups = await GetExpoPushTokens(command.UserIds);
        var pushTickets = new List<PushTicketRequest>();
            
        foreach (var translationGroup in translationGroups)
        {
            var pushTicket = new PushTicketRequest
            {
                PushTo = translationGroup.PushTokens,
                PushTitle = command.Title.Translate(translation, translationGroup.Culture),
                PushBody = command.Body.Translate(translation, translationGroup.Culture)
            };
            pushTickets.Add(pushTicket);
        }

        var responses = new List<PushTicketResponse>();
        
        try
        {
            foreach (var pushTicket in pushTickets)
            {
                var result = await client.SendPushAsync(pushTicket);
                responses.Add(result);
            }
        }
        catch (Exception e)
        {
            logger.LogError("Push Notification failed! Message: {Message}", e.Message);
        }
        
        var ticketsWithErrors = responses
            .Where(r => r.PushTicketErrors.Count > 0)
            .ToList();
        
        if (ticketsWithErrors.Count > 0)
        {
            foreach (var errorTicket in ticketsWithErrors)
            {
                var error = string.Join(", \n", errorTicket.PushTicketErrors
                    .Select(e => $"Code: ${e.ErrorCode} - ${e.ErrorMessage}"));
                
                logger.LogError("Push Notification failed! Errors: {ErrorList}", error);
            }
            return;
        }

        var failedTickets = responses.SelectMany(r => r.PushTicketStatuses
                .Where(t => t.TicketStatus != "ok")
                .ToList())
            .ToList();
            
        if (failedTickets.Count > 0)
        {
            foreach (var ticket in failedTickets)
            {
                logger.LogError("A push had failed! Message {Message}", ticket.TicketMessage);
            }
            return;
        }
            
        logger.LogInformation("All push notifications where send successfully");
    }

    private async Task<List<TranslationGroup>> GetExpoPushTokens(int[] userIds)
    {
        var context = await contextFactory.CreateDbContextAsync();

        return (await context.PushUpSettings
                .Where(t => userIds.Contains(t.UserId))
                .Where(t => t.Token != null)
                .Where(t => t.Enabled)
                .GroupBy(t => t.LanguageCode)
                .ToListAsync())
            .Select(s => new TranslationGroup(
                s.Select(t => t.Token).ToList()!, s.Key))
            .ToList();
    }

    private record TranslationGroup(List<string> PushTokens, CultureInfo Culture);
}