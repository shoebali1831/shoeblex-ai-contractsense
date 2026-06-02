import type { DocumentAnalysis } from '../types'
import { ClauseCard } from './ClauseCard'
import { RiskFindingCard } from './RiskFindingCard'
import { RiskScoreCard } from './RiskScoreCard'

type AnalysisPanelProps = {
  analysis?: DocumentAnalysis
}

export function AnalysisPanel({ analysis }: AnalysisPanelProps) {
  const clauses = analysis?.clauses || []
  const risks = analysis?.risks || []

  return (
    <div className="panel">
      <div className="panel-header">
        <h3>AI Analysis</h3>
        <span className="status-pill">{analysis?.status || 'Pending'}</span>
      </div>
      <RiskScoreCard score={analysis?.riskScore} level={analysis?.riskLevel} />

      <div className="card">
        <h4>Summary</h4>
        <p className="body-copy">
          {analysis?.summary || 'No analysis available yet for this document.'}
        </p>
      </div>

      <h4 className="section-title">Clauses</h4>
      <div className="stack">
        {clauses.map((clause) => (
          <ClauseCard key={`${clause.title}-${clause.pageNumber}`} clause={clause} />
        ))}
        {clauses.length === 0 ? (
          <div className="card empty-state">
            <p className="muted">No clauses extracted for this document yet.</p>
          </div>
        ) : null}
      </div>

      <h4 className="section-title">Risk Findings</h4>
      <div className="stack">
        {risks.map((finding) => (
          <RiskFindingCard
            key={`${finding.riskTitle}-${finding.pageNumber}`}
            finding={finding}
          />
        ))}
        {risks.length === 0 ? (
          <div className="card empty-state">
            <p className="muted">No risk findings were returned by the analysis.</p>
          </div>
        ) : null}
      </div>
    </div>
  )
}
