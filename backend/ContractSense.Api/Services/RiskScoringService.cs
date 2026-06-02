using ContractSense.Api.Models.Entities;

namespace ContractSense.Api.Services;

public class RiskScoringService : IRiskScoringService
{
    public (int score, string level) Calculate(IReadOnlyCollection<RiskFinding> findings)
    {
        if (findings.Count == 0)
        {
            return (0, "Low");
        }

        var weighted = findings.Sum(item => item.Severity switch
        {
            "Critical" => 25,
            "High" => 18,
            "Medium" => 10,
            _ => 4
        });

        var score = Math.Clamp(weighted, 0, 100);
        var level = score switch
        {
            <= 30 => "Low",
            <= 70 => "Medium",
            _ => "High"
        };

        return (score, level);
    }
}
