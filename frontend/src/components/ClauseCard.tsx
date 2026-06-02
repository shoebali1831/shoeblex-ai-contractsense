import type { Clause } from '../types'

type ClauseCardProps = {
  clause: Clause
}

export function ClauseCard({ clause }: ClauseCardProps) {
  return (
    <div className="card compact insight-card">
      <div className="panel-header">
        <h4>{clause.title}</h4>
        <span className={`severity-chip severity-${clause.riskLevel.toLowerCase()}`}>
          {clause.riskLevel}
        </span>
      </div>
      <p className="muted meta-line">
        {clause.clauseType} · Page {clause.pageNumber}
      </p>
      <p className="body-copy">{clause.summary}</p>
      <p className="muted body-copy">
        Why flagged: {clause.riskReason}
      </p>
    </div>
  )
}
