using ContractSense.Api.Models.Entities;
using ContractSense.Api.Services;

namespace ContractSense.Api.Tests;

public class RiskScoringServiceTests
{
    private readonly RiskScoringService _service = new();

    [Fact]
    public void Calculate_ReturnsLow_WhenNoFindings()
    {
        var result = _service.Calculate([]);

        Assert.Equal(0, result.score);
        Assert.Equal("Low", result.level);
    }

    [Fact]
    public void Calculate_ReturnsExpectedWeightedScore_AndLevel()
    {
        var findings = new List<RiskFinding>
        {
            new() { Severity = "Critical" },
            new() { Severity = "Medium" },
            new() { Severity = "Low" }
        };

        var result = _service.Calculate(findings);

        Assert.Equal(39, result.score);
        Assert.Equal("Medium", result.level);
    }

    [Fact]
    public void Calculate_ClampsScoreTo100_WhenFindingsExceedRange()
    {
        var findings = Enumerable.Range(0, 10)
            .Select(_ => new RiskFinding { Severity = "Critical" })
            .ToList();

        var result = _service.Calculate(findings);

        Assert.Equal(100, result.score);
        Assert.Equal("High", result.level);
    }
}
