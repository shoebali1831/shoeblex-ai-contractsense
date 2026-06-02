using ContractSense.Api.Models.Dto;

namespace ContractSense.Api.Services;

public interface IDocumentService
{
    Task<UploadResponseDto> UploadAsync(IFormFile file, CancellationToken cancellationToken);
    Task<DocumentDto?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken);
    Task<FileStream?> GetDocumentFileAsync(Guid documentId, CancellationToken cancellationToken);
}
