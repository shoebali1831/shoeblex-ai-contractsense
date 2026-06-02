namespace ContractSense.Api.Models.Entities;

public class Clause
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public string ClauseType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Low";
    public string RiskReason { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string SourceText { get; set; } = string.Empty;
}
