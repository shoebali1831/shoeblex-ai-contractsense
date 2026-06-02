using ContractSense.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractSense.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<ContractChunk> ContractChunks => Set<ContractChunk>();
    public DbSet<Clause> Clauses => Set<Clause>();
    public DbSet<RiskFinding> RiskFindings => Set<RiskFinding>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OriginalFileName).HasMaxLength(255);
            entity.Property(x => x.StoredFilePath).HasMaxLength(512);
            entity.Property(x => x.Status).HasMaxLength(64);
            entity.Property(x => x.RiskLevel).HasMaxLength(32);
        });

        modelBuilder.Entity<ContractChunk>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Content).HasColumnType("text");
            entity.Property(x => x.Embedding).HasColumnType("vector(1536)");
            entity.HasIndex(x => new { x.DocumentId, x.ChunkIndex });
        });

        modelBuilder.Entity<Clause>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ClauseType).HasMaxLength(128);
            entity.Property(x => x.Title).HasMaxLength(256);
            entity.Property(x => x.RiskLevel).HasMaxLength(32);
        });

        modelBuilder.Entity<RiskFinding>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RiskTitle).HasMaxLength(256);
            entity.Property(x => x.Severity).HasMaxLength(32);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Question).HasColumnType("text");
            entity.Property(x => x.Answer).HasColumnType("text");
        });
    }
}
