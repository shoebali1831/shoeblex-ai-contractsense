import type { RiskFinding } from '../types'

type RiskFindingCardProps = {
  finding: RiskFinding
}

export function RiskFindingCard({ finding }: RiskFindingCardProps) {
  return (
    <div className="card compact insight-card">
      <div className="panel-header">
        <h4>{finding.riskTitle}</h4>
        <span className={`severity-chip severity-${finding.severity.toLowerCase()}`}>
          {finding.severity}
        </span>
      </div>
      <p className="muted meta-line">Page {finding.pageNumber}</p>
      <p className="body-copy">{finding.explanation}</p>
      <p className="muted body-copy">
        <strong>Recommendation:</strong> {finding.recommendation}
      </p>
    </div>
  )
}
