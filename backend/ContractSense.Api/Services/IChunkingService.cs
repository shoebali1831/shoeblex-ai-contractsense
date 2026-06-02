using ContractSense.Api.Models.Entities;
using ContractSense.Api.Models.Internal;

namespace ContractSense.Api.Services;

public interface IChunkingService
{
    Task GenerateAndStoreChunksAsync(Document document, IReadOnlyCollection<PageText> pages, CancellationToken cancellationToken);
}
