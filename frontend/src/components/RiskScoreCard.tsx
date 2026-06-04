type RiskScoreCardProps = {
  score?: number
  level?: string
}

export function RiskScoreCard({ score, level }: RiskScoreCardProps) {
  const numericScore = score ?? 0
  const levelText = level ?? 'Not analyzed yet'
  const safeWidth = Math.min(100, Math.max(0, numericScore))

  return (
    <div className="card risk-card">
      <div className="risk-header">
        <h4>Risk Score</h4>
        <span className={`severity-chip severity-${(level || 'low').toLowerCase()}`}>
          {levelText}
        </span>
      </div>
      <p className="risk-score">{score ?? '--'}</p>
      <div className="risk-progress-track" role="presentation">
        <span className="risk-progress-fill" style={{ width: `${safeWidth}%` }} />
      </div>
      <div className="risk-scale">
        <span>0 Low</span>
        <span>30 Medium</span>
        <span>70 High</span>
        <span>100</span>
      </div>
      <p className="risk-level">Higher score means more contractual risk signals.</p>
    </div>
  )
}
