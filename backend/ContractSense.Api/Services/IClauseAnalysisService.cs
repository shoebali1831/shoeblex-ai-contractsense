using ContractSense.Api.Models.Dto;
using ContractSense.Api.Models.Entities;

namespace ContractSense.Api.Services;

public interface IClauseAnalysisService
{
    Task<DocumentAnalysisResponseDto> AnalyzeDocumentAsync(Document document, CancellationToken cancellationToken);
}
