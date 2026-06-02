using ContractSense.Api.Data;
using ContractSense.Api.Models.Dto;
using ContractSense.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContractSense.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(
    IDocumentService documentService,
    ApplicationDbContext dbContext) : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<UploadResponseDto>> Upload([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var result = await documentService.UploadAsync(file, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(Error("invalid_request", ex.Message));
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, Error(
                "ai_provider_error",
                "Document processing failed while calling the AI provider. Check API key and provider configuration."));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, Error(
                "upload_processing_error",
                "Unexpected error occurred during document upload and processing."));
        }
    }

    [HttpGet("{documentId:guid}")]
    public async Task<ActionResult<DocumentDto>> GetById([FromRoute] Guid documentId, CancellationToken cancellationToken)
    {
        var result = await documentService.GetByIdAsync(documentId, cancellationToken);
        if (result is null)
        {
            return NotFound(Error("document_not_found", "Document not found."));
        }

        return Ok(result);
    }

    [HttpGet("{documentId:guid}/file")]
    public async Task<IActionResult> GetFile([FromRoute] Guid documentId, CancellationToken cancellationToken)
    {
        var fileStream = await documentService.GetDocumentFileAsync(documentId, cancellationToken);
        if (fileStream is null)
        {
            return NotFound(Error("document_file_not_found", "Document file not found."));
        }

        return File(fileStream, "application/pdf");
    }

    [HttpGet("{documentId:guid}/analysis")]
    public async Task<ActionResult<DocumentAnalysisResponseDto>> GetAnalysis([FromRoute] Guid documentId, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);
        if (document is null)
        {
            return NotFound(Error("document_not_found", "Document not found."));
        }

        var clauses = await dbContext.Clauses.Where(c => c.DocumentId == documentId).ToListAsync(cancellationToken);
        var risks = await dbContext.RiskFindings.Where(r => r.DocumentId == documentId).ToListAsync(cancellationToken);

        var response = new DocumentAnalysisResponseDto
        {
            DocumentId = document.Id,
            Status = document.Status,
            RiskScore = document.RiskScore,
            RiskLevel = document.RiskLevel,
            Summary = (document.ExtractedText ?? string.Empty)[..Math.Min(700, (document.ExtractedText ?? string.Empty).Length)],
            Clauses = clauses.Select(c => new ClauseDto
            {
                ClauseType = c.ClauseType,
                Title = c.Title,
                Summary = c.Summary,
                RiskLevel = c.RiskLevel,
                RiskReason = c.RiskReason,
                PageNumber = c.PageNumber,
                SourceText = c.SourceText
            }).ToList(),
            Risks = risks.Select(r => new RiskFindingDto
            {
                RiskTitle = r.RiskTitle,
                Severity = r.Severity,
                Explanation = r.Explanation,
                Recommendation = r.Recommendation,
                PageNumber = r.PageNumber,
                SourceText = r.SourceText
            }).ToList()
        };

        return Ok(response);
    }

    private ApiErrorResponseDto Error(string code, string message)
    {
        return new ApiErrorResponseDto
        {
            Code = code,
            Message = message,
            TraceId = HttpContext.TraceIdentifier
        };
    }
}
