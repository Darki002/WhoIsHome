using System.Text.Json.Serialization;

namespace WhoIsHome.External.PushUp.ApiClient;

public class PushTicketResponse
{
    [JsonPropertyName("data")]
    public List<PushTicketStatus> PushTicketStatuses { get; set; } = [];

    [JsonPropertyName("errors")]
    public List<PushTicketErrors> PushTicketErrors { get; set; } = [];

}

public class PushTicketStatus {

    /// <summary>
    /// "error" | "ok",
    /// </summary>
    [JsonPropertyName("status")]
    public string? TicketStatus { get; set; }

    [JsonPropertyName("id")]
    public string? TicketId { get; set; }

    [JsonPropertyName("message")]
    public string? TicketMessage { get; set; }

    [JsonPropertyName("details")]
    public object? TicketDetails { get; set; }
}

public class PushTicketErrors 
{
    [JsonPropertyName("code")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string? ErrorMessage { get; set; }
}