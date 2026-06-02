using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ContractSense.Api.Services;

public class OpenAiService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IOpenAiService
{
    private readonly string _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
    private readonly string _chatModel = configuration["OpenAI:ChatModel"] ?? "gpt-4o-mini";
    private readonly string _embeddingModel = configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
    private readonly string _baseUrl = (configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/v1").TrimEnd('/');
    private readonly string _httpReferer = configuration["OpenAI:HttpReferer"] ?? string.Empty;
    private readonly string _appTitle = configuration["OpenAI:XTitle"] ?? string.Empty;

    public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken)
    {
        EnsureKey();

        var payload = JsonSerializer.Serialize(new
        {
            model = _embeddingModel,
            input = text
        });

        using var request = CreateAuthorizedRequest(HttpMethod.Post, $"{_baseUrl}/embeddings");
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var client = httpClientFactory.CreateClient();
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        using var json = JsonDocument.Parse(body);
        var embeddingValues = json.RootElement.GetProperty("data")[0].GetProperty("embedding");

        var output = new float[embeddingValues.GetArrayLength()];
        var index = 0;
        foreach (var item in embeddingValues.EnumerateArray())
        {
            output[index++] = item.GetSingle();
        }

        return output;
    }

    public async Task<string> GetChatCompletionAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        EnsureKey();

        var payload = JsonSerializer.Serialize(new
        {
            model = _chatModel,
            temperature = 0.2,
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            }
        });

        using var request = CreateAuthorizedRequest(HttpMethod.Post, $"{_baseUrl}/chat/completions");
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var client = httpClientFactory.CreateClient();
        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        using var json = JsonDocument.Parse(body);
        return json.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()?
            .Trim() ?? string.Empty;
    }

    private void EnsureKey()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("OpenAI API key is missing. Set OpenAI:ApiKey in configuration.");
        }
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        if (!string.IsNullOrWhiteSpace(_httpReferer))
        {
            request.Headers.TryAddWithoutValidation("HTTP-Referer", _httpReferer);
        }

        if (!string.IsNullOrWhiteSpace(_appTitle))
        {
            request.Headers.TryAddWithoutValidation("X-Title", _appTitle);
        }

        return request;
    }
}
