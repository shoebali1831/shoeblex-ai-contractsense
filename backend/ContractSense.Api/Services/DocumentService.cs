using ContractSense.Api.Data;
using ContractSense.Api.Models.Dto;
using ContractSense.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractSense.Api.Services;

public class DocumentService(
    ApplicationDbContext dbContext,
    IPdfExtractionService pdfExtractionService,
    IChunkingService chunkingService,
    IClauseAnalysisService clauseAnalysisService) : IDocumentService
{
    public async Task<UploadResponseDto> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            throw new InvalidOperationException("Uploaded file is empty.");
        }

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
            file.ContentType != "application/pdf")
        {
            throw new InvalidOperationException("Only PDF files are supported.");
        }

        var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsRoot);

        var storedFileName = $"{Guid.NewGuid():N}.pdf";
        var storedPath = Path.Combine(uploadsRoot, storedFileName);

        await using (var stream = File.Create(storedPath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var document = new Document
        {
            OriginalFileName = file.FileName,
            StoredFilePath = storedPath,
            Status = "Uploaded"
        };

        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            document.Status = "Extracting";
            await dbContext.SaveChangesAsync(cancellationToken);

            var pages = await pdfExtractionService.ExtractPagesAsync(document, cancellationToken);
            document.ExtractedText = string.Join(Environment.NewLine + Environment.NewLine, pages.Select(p => $"Page {p.PageNumber}: {p.Text}"));

            document.Status = "Chunking";
            await dbContext.SaveChangesAsync(cancellationToken);

            await chunkingService.GenerateAndStoreChunksAsync(document, pages, cancellationToken);

            document.Status = "Analyzing";
            await dbContext.SaveChangesAsync(cancellationToken);

            await clauseAnalysisService.AnalyzeDocumentAsync(document, cancellationToken);
        }
        catch
        {
            document.Status = "Failed";
            await dbContext.SaveChangesAsync(cancellationToken);
            throw;
        }

        return new UploadResponseDto
        {
            DocumentId = document.Id,
            FileName = document.OriginalFileName,
            Status = document.Status,
            Message = "PDF uploaded and processed successfully."
        };
    }

    public async Task<DocumentDto?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        return new DocumentDto
        {
            DocumentId = document.Id,
            FileName = document.OriginalFileName,
            Status = document.Status,
            RiskScore = document.RiskScore,
            RiskLevel = document.RiskLevel,
            CreatedAt = document.CreatedAt
        };
    }

    public async Task<FileStream?> GetDocumentFileAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);
        if (document is null || !File.Exists(document.StoredFilePath))
        {
            return null;
        }

        return File.OpenRead(document.StoredFilePath);
    }
}
