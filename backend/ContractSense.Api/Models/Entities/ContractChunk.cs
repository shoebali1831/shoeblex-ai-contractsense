using Pgvector;

namespace ContractSense.Api.Models.Entities;

public class ContractChunk
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public int PageNumber { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public Vector Embedding { get; set; } = new(new float[1536]);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
