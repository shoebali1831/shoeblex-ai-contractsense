namespace ContractSense.Api.Services;

public interface IOpenAiService
{
    Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken);
    Task<string> GetChatCompletionAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken);
}
