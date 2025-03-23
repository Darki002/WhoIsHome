using Newtonsoft.Json;

namespace WhoIsHome.External.PushUp.ApiClient;

[JsonObject (MemberSerialization.OptIn)]
public class PushTicketResponse
{
    [JsonProperty (PropertyName = "data")]
    public List<PushTicketStatus> PushTicketStatuses { get; set; } = [];

    [JsonProperty (PropertyName = "errors")]
    public List<PushTicketErrors> PushTicketErrors { get; set; } = [];

}

[JsonObject (MemberSerialization.OptIn)]
public class PushTicketStatus {

    /// <summary>
    /// "error" | "ok",
    /// </summary>
    [JsonProperty (PropertyName = "status")] 
    public string? TicketStatus { get; set; }

    [JsonProperty (PropertyName = "id")]
    public string? TicketId { get; set; }

    [JsonProperty (PropertyName = "message")]
    public string? TicketMessage { get; set; }

    [JsonProperty (PropertyName = "details")]
    public object? TicketDetails { get; set; }
}

[JsonObject (MemberSerialization.OptIn)]
public class PushTicketErrors 
{
    [JsonProperty (PropertyName = "code")]
    public string? ErrorCode { get; set; }

    [JsonProperty (PropertyName = "message")]
    public string? ErrorMessage { get; set; }
}