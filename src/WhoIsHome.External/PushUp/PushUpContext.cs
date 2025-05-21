using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhoIsHome.External.PushUp.ApiClient;
using WhoIsHome.Shared.Configurations;

namespace WhoIsHome.External.PushUp;

public class PushUpContext(
    PushApiClient client, 
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    IConfiguration configuration,
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
        try
        {
            var pushTokens = await GetExpoPushTokens(command.UserIds);
            
            var pushTicket = new PushTicketRequest
            {
                PushTo = pushTokens,
                PushTitle = command.Title, // TODO: translate foreach user
                PushBody = command.Body
            };
            var result = await client.SendPushAsync(pushTicket);

            if (result.PushTicketErrors.Count > 0)
            {
                var error = string.Join(", \n", result.PushTicketErrors
                    .Select(e => $"Code: ${e.ErrorCode} - ${e.ErrorMessage}"));
                
                logger.LogError("Push Notification failed! Errors: {ErrorList}", error);
                return;
            }

            var failedTickets = result.PushTicketStatuses
                .Where(t => t.TicketStatus != "ok")
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
        catch (Exception e)
        {
            logger.LogError("Push Notification failed! Message: {Message}", e.Message);
        }
    }

    private async Task<List<string>> GetExpoPushTokens(int[] userIds)
    {
        var context = await contextFactory.CreateDbContextAsync();

        return (await context.PushUpSettings
                .Where(t => userIds.Contains(t.UserId))
                .Where(t => t.Token != null)
                .Where(t => t.Enabled)
                .ToListAsync())
            .Select(t => t.Token)
            .Cast<string>()
            .ToList();
    }
}