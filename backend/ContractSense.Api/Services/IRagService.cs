using ContractSense.Api.Models.Dto;

namespace ContractSense.Api.Services;

public interface IRagService
{
    Task<AskQuestionResponseDto> AskAsync(Guid documentId, string question, CancellationToken cancellationToken);
}
