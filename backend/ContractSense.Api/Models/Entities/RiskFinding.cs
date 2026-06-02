namespace ContractSense.Api.Models.Entities;

public class RiskFinding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public string RiskTitle { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string Explanation { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string SourceText { get; set; } = string.Empty;
}
