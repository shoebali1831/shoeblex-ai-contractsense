using System.Text.Json;
using ContractSense.Api.Data;
using ContractSense.Api.Models.Dto;
using ContractSense.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractSense.Api.Services;

public class ClauseAnalysisService(
    ApplicationDbContext dbContext,
    IOpenAiService openAiService,
    IRiskScoringService riskScoringService) : IClauseAnalysisService
{
    public async Task<DocumentAnalysisResponseDto> AnalyzeDocumentAsync(Document document, CancellationToken cancellationToken)
    {
        var chunks = await dbContext.ContractChunks
            .Where(c => c.DocumentId == document.Id)
            .OrderBy(c => c.ChunkIndex)
            .ToListAsync(cancellationToken);

        if (chunks.Count == 0)
        {
            throw new InvalidOperationException("No chunks found for clause and risk analysis.");
        }

        dbContext.Clauses.RemoveRange(dbContext.Clauses.Where(c => c.DocumentId == document.Id));
        dbContext.RiskFindings.RemoveRange(dbContext.RiskFindings.Where(r => r.DocumentId == document.Id));
        await dbContext.SaveChangesAsync(cancellationToken);

        var context = string.Join(Environment.NewLine + Environment.NewLine, chunks.Take(16).Select(c => $"[Page {c.PageNumber}] {c.Content}"));
        var prompt = """
            Extract key legal clauses and risk findings from the contract context.
            Return strict JSON with this shape:
            {
              "clauses":[{"clauseType":"","title":"","summary":"","riskLevel":"Low|Medium|High|Critical","riskReason":"","pageNumber":0,"sourceText":""}],
              "risks":[{"riskTitle":"","severity":"Low|Medium|High|Critical","explanation":"","recommendation":"","pageNumber":0,"sourceText":""}]
            }
            Keep entries concise and practical.
            """;

        var raw = await openAiService.GetChatCompletionAsync(
            "You are a legal contract analysis assistant. Return valid JSON only.",
            $"{prompt}\n\nCONTRACT CONTEXT:\n{context}",
            cancellationToken);

        var (clauses, risks) = ParseOrFallback(raw, chunks);

        if (clauses.Count > 0)
        {
            dbContext.Clauses.AddRange(clauses);
        }

        if (risks.Count > 0)
        {
            dbContext.RiskFindings.AddRange(risks);
        }

        var (score, level) = riskScoringService.Calculate(risks);
        document.RiskScore = score;
        document.RiskLevel = level;
        document.Status = "Analyzed";

        await dbContext.SaveChangesAsync(cancellationToken);

        return new DocumentAnalysisResponseDto
        {
            DocumentId = document.Id,
            Status = document.Status,
            RiskScore = score,
            RiskLevel = level,
            Summary = BuildSummary(chunks),
            Clauses = clauses.Select(c => new ClauseDto
            {
                ClauseType = c.ClauseType,
                Title = c.Title,
                Summary = c.Summary,
                RiskLevel = c.RiskLevel,
                RiskReason = c.RiskReason,
                PageNumber = c.PageNumber,
                SourceText = c.SourceText
            }).ToList(),
            Risks = risks.Select(r => new RiskFindingDto
            {
                RiskTitle = r.RiskTitle,
                Severity = r.Severity,
                Explanation = r.Explanation,
                Recommendation = r.Recommendation,
                PageNumber = r.PageNumber,
                SourceText = r.SourceText
            }).ToList()
        };
    }

    private static (List<Clause> clauses, List<RiskFinding> risks) ParseOrFallback(string raw, IReadOnlyList<ContractChunk> chunks)
    {
        try
        {
            var cleaned = ExtractJsonPayload(raw);
            using var json = JsonDocument.Parse(cleaned);
            var clauses = json.RootElement.GetProperty("clauses")
                .EnumerateArray()
                .Select(item => new Clause
                {
                    ClauseType = item.GetProperty("clauseType").GetString() ?? "General",
                    Title = item.GetProperty("title").GetString() ?? "Clause",
                    Summary = item.GetProperty("summary").GetString() ?? string.Empty,
                    RiskLevel = NormalizeRiskLevel(item.GetProperty("riskLevel").GetString()),
                    RiskReason = item.GetProperty("riskReason").GetString() ?? string.Empty,
                    PageNumber = Math.Max(1, item.GetProperty("pageNumber").GetInt32()),
                    SourceText = item.GetProperty("sourceText").GetString() ?? string.Empty
                }).ToList();

            var risks = json.RootElement.GetProperty("risks")
                .EnumerateArray()
                .Select(item => new RiskFinding
                {
                    RiskTitle = item.GetProperty("riskTitle").GetString() ?? "Risk",
                    Severity = NormalizeRiskLevel(item.GetProperty("severity").GetString()),
                    Explanation = item.GetProperty("explanation").GetString() ?? string.Empty,
                    Recommendation = item.GetProperty("recommendation").GetString() ?? string.Empty,
                    PageNumber = Math.Max(1, item.GetProperty("pageNumber").GetInt32()),
                    SourceText = item.GetProperty("sourceText").GetString() ?? string.Empty
                }).ToList();

            return (clauses, risks);
        }
        catch
        {
            var fallbackClause = chunks.Take(3).Select(chunk => new Clause
            {
                ClauseType = "General",
                Title = "Extracted Contract Clause",
                Summary = chunk.Content[..Math.Min(250, chunk.Content.Length)],
                RiskLevel = "Medium",
                RiskReason = "Auto-fallback analysis due to invalid model JSON output.",
                PageNumber = chunk.PageNumber,
                SourceText = chunk.Content[..Math.Min(400, chunk.Content.Length)]
            }).ToList();

            var fallbackRisk = chunks.Where(c =>
                    c.Content.Contains("terminate", StringComparison.OrdinalIgnoreCase) ||
                    c.Content.Contains("liability", StringComparison.OrdinalIgnoreCase) ||
                    c.Content.Contains("penalty", StringComparison.OrdinalIgnoreCase))
                .Take(3)
                .Select(chunk => new RiskFinding
                {
                    RiskTitle = "Potential contractual risk",
                    Severity = "Medium",
                    Explanation = "Potentially sensitive term detected by keyword heuristic.",
                    Recommendation = "Review this section carefully.",
                    PageNumber = chunk.PageNumber,
                    SourceText = chunk.Content[..Math.Min(400, chunk.Content.Length)]
                }).ToList();

            return (fallbackClause, fallbackRisk);
        }
    }

    private static string ExtractJsonPayload(string raw)
    {
        var trimmed = raw.Trim();

        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNewLine = trimmed.IndexOf('\n');
            if (firstNewLine >= 0)
            {
                trimmed = trimmed[(firstNewLine + 1)..];
            }

            var closingFenceIndex = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (closingFenceIndex >= 0)
            {
                trimmed = trimmed[..closingFenceIndex];
            }
        }

        var firstBrace = trimmed.IndexOf('{');
        var lastBrace = trimmed.LastIndexOf('}');
        if (firstBrace >= 0 && lastBrace >= firstBrace)
        {
            return trimmed[firstBrace..(lastBrace + 1)];
        }

        return trimmed;
    }

    private static string NormalizeRiskLevel(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "critical" => "Critical",
            "high" => "High",
            "medium" => "Medium",
            _ => "Low"
        };
    }

    private static string BuildSummary(IReadOnlyCollection<ContractChunk> chunks)
    {
        var first = chunks.OrderBy(c => c.ChunkIndex).FirstOrDefault()?.Content ?? string.Empty;
        return first[..Math.Min(500, first.Length)];
    }
}
