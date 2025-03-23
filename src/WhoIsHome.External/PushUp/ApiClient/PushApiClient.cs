using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace WhoIsHome.External.PushUp.ApiClient;

public static class PushApiClient
{
    private const string ExpoBackendHost = "https://exp.host";
    private const string PushSendPath = "/--/api/v2/push/send";
    private const string PushGetReceiptsPath = "/--/api/v2/push/getReceipts";

    private static readonly HttpClientHandler HttpHandler = new() { MaxConnectionsPerServer = 6 };
    private static readonly HttpClient HttpClient = new(HttpHandler);

    static PushApiClient()
    {
        HttpClient.BaseAddress = new Uri(ExpoBackendHost);
        HttpClient.DefaultRequestHeaders.Accept.Clear();
        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }
    
    public static void SetAccessToken(string value)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
    }

    public static async Task<PushTicketResponse?> PushSendAsync(PushTicketRequest pushTicketRequest)
    {
        var ticketResponse = await PostAsync<PushTicketRequest, PushTicketResponse>(pushTicketRequest, PushSendPath);
        return ticketResponse;
    }

    public static async Task<PushReceiptResponse?> PushGetReceiptsAsync(PushReceiptRequest pushReceiptRequest)
    {
        var receiptResponse =
            await PostAsync<PushReceiptRequest, PushReceiptResponse>(pushReceiptRequest, PushGetReceiptsPath);
        return receiptResponse;
    }

    private static async Task<TResponse?> PostAsync<TRequest, TResponse>(TRequest requestObj, string path) where TRequest : new()
    {
        var serializedRequestObj = JsonConvert.SerializeObject(requestObj, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        var requestBody = new StringContent(serializedRequestObj, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync(path, requestBody);

        if (!response.IsSuccessStatusCode) return default;
        
        var rawResponseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(rawResponseBody);
    }
}