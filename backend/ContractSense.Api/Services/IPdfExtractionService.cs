using ContractSense.Api.Models.Entities;
using ContractSense.Api.Models.Internal;

namespace ContractSense.Api.Services;

public interface IPdfExtractionService
{
    Task<List<PageText>> ExtractPagesAsync(Document document, CancellationToken cancellationToken);
}
