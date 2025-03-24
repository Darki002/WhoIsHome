using System.Text.Json.Serialization;

namespace WhoIsHome.External.PushUp.ApiClient;

public class PushReceiptRequest
{
    [JsonPropertyName("ids")]
    public List<string> PushTicketIds { get; set; } = [];
}