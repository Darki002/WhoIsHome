using Newtonsoft.Json;

namespace WhoIsHome.External.PushUp.ApiClient;

[JsonObject(MemberSerialization.OptIn)]
public class PushReceiptResponse
{
    [JsonProperty(PropertyName = "data")]
    public Dictionary<string, PushTicketDeliveryStatus> PushTicketReceipts { get; set; } = [];

    [JsonProperty(PropertyName = "errors")]
    public List<PushReceiptErrorInformation> ErrorInformations { get; set; } = [];
}

[JsonObject(MemberSerialization.OptIn)]
public class PushTicketDeliveryStatus
{
    [JsonProperty(PropertyName = "status")]
    public required string DeliveryStatus { get; set; }

    [JsonProperty(PropertyName = "message")]
    public required string DeliveryMessage { get; set; }

    [JsonProperty(PropertyName = "details")]
    public required object DeliveryDetails { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
public class PushReceiptErrorInformation
{
    [JsonProperty(PropertyName = "code")]
    public required string ErrorCode { get; set; }

    [JsonProperty(PropertyName = "message")]
    public required string ErrorMessage { get; set; }
}