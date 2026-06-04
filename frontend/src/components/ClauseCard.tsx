import type { Clause } from '../types'

type ClauseCardProps = {
  clause: Clause
  onJumpToPage?: (page: number) => void
}

export function ClauseCard({ clause, onJumpToPage }: ClauseCardProps) {
  return (
    <div className="card compact insight-card">
      <div className="panel-header">
        <h4>{clause.title}</h4>
        <span className={`severity-chip severity-${clause.riskLevel.toLowerCase()}`}>
          {clause.riskLevel}
        </span>
      </div>
      <div className="meta-row">
        <span className="meta-tag">{clause.clauseType}</span>
        <span className="meta-tag">Page {clause.pageNumber}</span>
      </div>
      <p className="body-copy">{clause.summary}</p>
      <p className="muted body-copy">
        Why flagged: {clause.riskReason}
      </p>
      {onJumpToPage ? (
        <button
          type="button"
          className="ghost-action"
          onClick={() => onJumpToPage(clause.pageNumber)}
        >
          Open page {clause.pageNumber}
        </button>
      ) : null}
    </div>
  )
}
