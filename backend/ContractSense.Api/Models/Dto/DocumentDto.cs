namespace ContractSense.Api.Models.Dto;

public class DocumentDto
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? RiskScore { get; set; }
    public string? RiskLevel { get; set; }
    public DateTime CreatedAt { get; set; }
}
