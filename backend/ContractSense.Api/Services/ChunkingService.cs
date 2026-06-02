using ContractSense.Api.Data;
using ContractSense.Api.Models.Entities;
using ContractSense.Api.Models.Internal;
using Pgvector;

namespace ContractSense.Api.Services;

public class ChunkingService(ApplicationDbContext dbContext, IOpenAiService openAiService) : IChunkingService
{
    private const int ChunkSize = 1200;
    private const int Overlap = 200;

    public async Task GenerateAndStoreChunksAsync(Document document, IReadOnlyCollection<PageText> pages, CancellationToken cancellationToken)
    {
        var chunks = new List<ContractChunk>();
        var chunkIndex = 0;

        foreach (var page in pages)
        {
            foreach (var chunkText in SplitWithOverlap(page.Text, ChunkSize, Overlap))
            {
                var embedding = await openAiService.GetEmbeddingAsync(chunkText, cancellationToken);

                chunks.Add(new ContractChunk
                {
                    DocumentId = document.Id,
                    PageNumber = page.PageNumber,
                    ChunkIndex = chunkIndex++,
                    Content = chunkText,
                    Embedding = new Vector(embedding)
                });
            }
        }

        if (chunks.Count == 0)
        {
            throw new InvalidOperationException("No chunks generated from extracted text.");
        }

        dbContext.ContractChunks.AddRange(chunks);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<string> SplitWithOverlap(string text, int chunkSize, int overlap)
    {
        var normalized = text.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            yield break;
        }

        var start = 0;
        while (start < normalized.Length)
        {
            var length = Math.Min(chunkSize, normalized.Length - start);
            yield return normalized.Substring(start, length).Trim();

            if (start + length >= normalized.Length)
            {
                break;
            }

            start += Math.Max(1, chunkSize - overlap);
        }
    }
}
