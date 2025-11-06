using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhoIsHome.External.PushUp.ApiClient;

public class PushApiClient
{
    private const string ExpoBackendHost = "https://exp.host";
    private const string PushSendPath = "/--/api/v2/push/send";
    private const string PushGetReceiptsPath = "/--/api/v2/push/getReceipts";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
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

    public async Task<PushTicketResponse> SendPushAsync(PushTicketRequest pushTicketRequest)
    {
        var ticketResponse = await PostAsync<PushTicketRequest, PushTicketResponse>(pushTicketRequest, PushSendPath);

        if (ticketResponse is null)
        {
            throw new HttpRequestException("No response from Request!");
        }
        
        return ticketResponse;
    }

    public async Task<PushReceiptResponse?> GetPushReceiptsAsync(PushReceiptRequest pushReceiptRequest)
    {
        var receiptResponse =
            await PostAsync<PushReceiptRequest, PushReceiptResponse>(pushReceiptRequest, PushGetReceiptsPath);
        return receiptResponse;
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(TRequest requestObj, string path) where TRequest : new()
    {
        var serializedRequestObj = JsonSerializer.Serialize(requestObj, JsonSerializerOptions);
    
        var requestBody = new StringContent(serializedRequestObj, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(path, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request for ${typeof(TRequest).Name} failed with {response.StatusCode}");
        }
        
        var rawResponseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(rawResponseBody)!;
    }
}