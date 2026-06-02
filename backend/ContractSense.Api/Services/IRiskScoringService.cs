using ContractSense.Api.Models.Entities;

namespace ContractSense.Api.Services;

public interface IRiskScoringService
{
    (int score, string level) Calculate(IReadOnlyCollection<RiskFinding> findings);
}
