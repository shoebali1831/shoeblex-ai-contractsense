namespace ContractSense.Api.Models.Dto;

public class ApiErrorResponseDto
{
    public string Code { get; set; } = "internal_error";
    public string Message { get; set; } = "An unexpected error occurred.";
    public string TraceId { get; set; } = string.Empty;
}
