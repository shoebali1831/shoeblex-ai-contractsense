using ContractSense.Api.Models.Dto;
using ContractSense.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractSense.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IRagService ragService) : ControllerBase
{
    [HttpPost("ask")]
    public async Task<ActionResult<AskQuestionResponseDto>> Ask([FromBody] AskQuestionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await ragService.AskAsync(request.DocumentId, request.Question, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Error("document_not_found", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(Error("invalid_request", ex.Message));
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, Error(
                "ai_provider_error",
                "Chat request failed while calling the AI provider. Check API key and provider configuration."));
        }
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
