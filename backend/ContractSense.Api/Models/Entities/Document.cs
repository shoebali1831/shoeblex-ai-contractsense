namespace ContractSense.Api.Models.Entities;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFilePath { get; set; } = string.Empty;
    public string Status { get; set; } = "Uploaded";
    public int? RiskScore { get; set; }
    public string? RiskLevel { get; set; }
    public string? ExtractedText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
