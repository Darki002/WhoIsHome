using System.Text.Json.Serialization;

namespace WhoIsHome.External.PushUp.ApiClient;

public class PushTicketRequest
{
    [JsonPropertyName("to")]
    public List<string> PushTo { get; set; } = [];

    [JsonPropertyName("data")]
    public object? PushData { get; set; }

    [JsonPropertyName("title")]
    public string? PushTitle { get; set; }

    [JsonPropertyName("body")]
    public string? PushBody { get; set; }

    [JsonPropertyName("ttl")]
    public int? PushTtl { get; set; }

    [JsonPropertyName("expiration")]
    public int? PushExpiration { get; set; }

    /// <summary>
    /// 'default' | 'normal' | 'high'
    /// </summary>
    [JsonPropertyName("priority")]
    public string? PushPriority { get; set; }

    [JsonPropertyName("subtitle")]
    public string? PushSubTitle { get; set; }

    /// <summary>
    /// 'default' | null
    /// </summary>
    [JsonPropertyName("sound")]
    public string? PushSound { get; set; }

    [JsonPropertyName("badge")]
    public int? PushBadgeCount { get; set; }

    [JsonPropertyName("channelId")]
    public string? PushChannelId { get; set; }

    [JsonPropertyName("categoryId")]
    public string? PushCategoryId { get; set; }
}