using Newtonsoft.Json;

namespace WhoIsHome.External.PushUp.ApiClient;

[JsonObject(MemberSerialization.OptIn)]
public class PushReceiptRequest
{

    [JsonProperty(PropertyName = "ids")] 
    public List<string> PushTicketIds { get; set; } = [];
}