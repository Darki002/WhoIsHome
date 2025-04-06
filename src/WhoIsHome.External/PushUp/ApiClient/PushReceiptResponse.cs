using System.Text.Json.Serialization;

namespace WhoIsHome.External.PushUp.ApiClient;

public class PushReceiptResponse
{
    [JsonPropertyName("data")]
    public Dictionary<string, PushTicketDeliveryStatus> PushTicketReceipts { get; set; } = [];

    [JsonPropertyName("errors")]
    public List<PushReceiptErrorInformation> ErrorInformations { get; set; } = [];
}

public class PushTicketDeliveryStatus
{
    [JsonPropertyName("status")]
    public required string DeliveryStatus { get; set; }

    [JsonPropertyName("message")]
    public required string DeliveryMessage { get; set; }

    [JsonPropertyName("details")]
    public required object DeliveryDetails { get; set; }
}

public class PushReceiptErrorInformation
{
    [JsonPropertyName("code")]
    public required string ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public required string ErrorMessage { get; set; }
}