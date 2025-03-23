using Newtonsoft.Json;

namespace WhoIsHome.External.PushUp.ApiClient;

[JsonObject(MemberSerialization.OptIn)]
public class PushTicketRequest
{
    [JsonProperty(PropertyName = "to")] 
    public List<string> PushTo { get; set; } = [];

    [JsonProperty(PropertyName = "data")]
    public object? PushData { get; set; }

    [JsonProperty(PropertyName = "title")]
    public string? PushTitle { get; set; }

    [JsonProperty(PropertyName = "body")]
    public string? PushBody { get; set; }

    [JsonProperty(PropertyName = "ttl")]
    public int? PushTtl { get; set; }

    [JsonProperty(PropertyName = "expiration")]
    public int? PushExpiration { get; set; }

    /// <summary>
    /// 'default' | 'normal' | 'high'
    /// </summary>
    [JsonProperty(PropertyName = "priority")] 
    public string? PushPriority { get; set; }

    [JsonProperty(PropertyName = "subtitle")]
    public string? PushSubTitle { get; set; }

    /// <summary>
    /// 'default' | null
    /// </summary>
    [JsonProperty(PropertyName = "sound")]	
    public string? PushSound { get; set; }

    [JsonProperty(PropertyName = "badge")]
    public int? PushBadgeCount { get; set; }

    [JsonProperty(PropertyName = "channelId")]
    public string? PushChannelId { get; set; }

    [JsonProperty(PropertyName = "categoryId")]
    public string? PushCategoryId { get; set; }
}