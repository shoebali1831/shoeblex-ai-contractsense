namespace ContractSense.Api.Models.Dto;

public class RiskFindingDto
{
    public string RiskTitle { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string Explanation { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string SourceText { get; set; } = string.Empty;
}
