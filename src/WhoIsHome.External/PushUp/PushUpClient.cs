using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.External.PushUp.ApiClient;

namespace WhoIsHome.External.PushUp;

public class PushUpClient(
    PushApiClient client, 
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    ILogger<PushApiClient> logger) 
    : IPushUpClient
{
    public void PushEventUpdate(PushUpEventUpdateCommand command, CancellationToken cancellationToken)
    {
        // We do not care about this Task. Fire and Forget, they will be sent in the background.
        // On Failure, we don't care, the user should not get an error on the Phone just because of this.
        _ = Task.Run(() => SendAsync(command), cancellationToken); // TODO: retry on failure?
    }

    private async Task SendAsync(PushUpEventUpdateCommand command)
    {
        try
        {
            var pushTokens = await GetExpoPushTokens(command.userIds);
            
            var pushTicket = new PushTicketRequest
            {
                PushTo = pushTokens,
                PushTitle = command.Title,
                PushBody = command.Body
            };
            var result = await client.SendPushAsync(pushTicket);

            if (result.PushTicketErrors.Count > 0)
            {
                var error = string.Join(", ", result.PushTicketErrors
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

        return (await context.ExpoPushTokens
                .Where(t => userIds.Contains(t.Id))
                .Where(t => t.Token != null)
                .ToListAsync())
            .Select(t => t.Token)
            .Cast<string>()
            .ToList();
    }
}