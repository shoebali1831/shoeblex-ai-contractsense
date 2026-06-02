namespace ContractSense.Api.Models.Dto;

public class DocumentAnalysisResponseDto
{
    public Guid DocumentId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? RiskScore { get; set; }
    public string? RiskLevel { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<ClauseDto> Clauses { get; set; } = [];
    public List<RiskFindingDto> Risks { get; set; } = [];
}
