using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhoIsHome.External.PushUp.ApiClient;

public class PushApiClient
{
    private const string ExpoBackendHost = "https://exp.host";
    private const string PushSendPath = "/--/api/v2/push/send";
    private const string PushGetReceiptsPath = "/--/api/v2/push/getReceipts";

    private readonly HttpClientHandler httpHandler = new() { MaxConnectionsPerServer = 6 };
    private readonly HttpClient httpClient;

    public PushApiClient()
    {
        httpClient = new HttpClient(httpHandler);
        httpClient.BaseAddress = new Uri(ExpoBackendHost);
        
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }
    
    public void SetAccessToken(string value)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
    }

    public async Task<PushTicketResponse?> PushSendAsync(PushTicketRequest pushTicketRequest)
    {
        var ticketResponse = await PostAsync<PushTicketRequest, PushTicketResponse>(pushTicketRequest, PushSendPath);
        return ticketResponse;
    }

    public async Task<PushReceiptResponse?> PushGetReceiptsAsync(PushReceiptRequest pushReceiptRequest)
    {
        var receiptResponse =
            await PostAsync<PushReceiptRequest, PushReceiptResponse>(pushReceiptRequest, PushGetReceiptsPath);
        return receiptResponse;
    }

    private async Task<TResponse?> PostAsync<TRequest, TResponse>(TRequest requestObj, string path) where TRequest : new()
    {
        var serializedRequestObj = JsonSerializer.Serialize(requestObj, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    
        var requestBody = new StringContent(serializedRequestObj, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(path, requestBody);

        if (!response.IsSuccessStatusCode) return default;
        
        var rawResponseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(rawResponseBody);
    }
}