import type { RiskFinding } from '../types'

type RiskFindingCardProps = {
  finding: RiskFinding
  onJumpToPage?: (page: number) => void
}

export function RiskFindingCard({ finding, onJumpToPage }: RiskFindingCardProps) {
  return (
    <div className="card compact insight-card">
      <div className="panel-header">
        <h4>{finding.riskTitle}</h4>
        <span className={`severity-chip severity-${finding.severity.toLowerCase()}`}>
          {finding.severity}
        </span>
      </div>
      <div className="meta-row">
        <span className="meta-tag">Page {finding.pageNumber}</span>
      </div>
      <p className="body-copy">{finding.explanation}</p>
      <p className="muted body-copy">
        <strong>Recommendation:</strong> {finding.recommendation}
      </p>
      {onJumpToPage ? (
        <button
          type="button"
          className="ghost-action"
          onClick={() => onJumpToPage(finding.pageNumber)}
        >
          Open page {finding.pageNumber}
        </button>
      ) : null}
    </div>
  )
}
