import { useEffect, useMemo, useState } from 'react'
import type { DocumentAnalysis } from '../types'
import { ClauseCard } from './ClauseCard'
import { RiskFindingCard } from './RiskFindingCard'
import { RiskScoreCard } from './RiskScoreCard'

type AnalysisPanelProps = {
  analysis?: DocumentAnalysis
  onJumpToPage?: (page: number) => void
}

const SEVERITY_FILTER_STORAGE_KEY = 'contractsense.analysis.severityFilter'
const CLAUSE_FILTER_STORAGE_KEY = 'contractsense.analysis.clauseTypeFilter'

export function AnalysisPanel({ analysis, onJumpToPage }: AnalysisPanelProps) {
  const clauses = analysis?.clauses || []
  const risks = analysis?.risks || []
  const [severityFilter, setSeverityFilter] = useState(
    () => window.localStorage.getItem(SEVERITY_FILTER_STORAGE_KEY) || 'All',
  )
  const [clauseTypeFilter, setClauseTypeFilter] = useState(
    () => window.localStorage.getItem(CLAUSE_FILTER_STORAGE_KEY) || 'All',
  )

  const filteredClauses = useMemo(
    () =>
      clauses.filter((clause) => {
        const severityPass =
          severityFilter === 'All' || clause.riskLevel === severityFilter
        const clauseTypePass =
          clauseTypeFilter === 'All' || clause.clauseType === clauseTypeFilter
        return severityPass && clauseTypePass
      }),
    [clauses, severityFilter, clauseTypeFilter],
  )

  const filteredRisks = useMemo(
    () =>
      risks.filter(
        (risk) => severityFilter === 'All' || risk.severity === severityFilter,
      ),
    [risks, severityFilter],
  )

  const availableClauseTypes = useMemo(
    () => ['All', ...Array.from(new Set(clauses.map((item) => item.clauseType)))],
    [clauses],
  )

  useEffect(() => {
    if (!availableClauseTypes.includes(clauseTypeFilter)) {
      setClauseTypeFilter('All')
    }
  }, [availableClauseTypes, clauseTypeFilter])

  useEffect(() => {
    window.localStorage.setItem(SEVERITY_FILTER_STORAGE_KEY, severityFilter)
  }, [severityFilter])

  useEffect(() => {
    window.localStorage.setItem(CLAUSE_FILTER_STORAGE_KEY, clauseTypeFilter)
  }, [clauseTypeFilter])

  const highRiskCount = risks.filter(
    (item) => item.severity === 'High' || item.severity === 'Critical',
  ).length

  return (
    <div className="panel">
      <div className="panel-header">
        <h3>AI Analysis</h3>
        <span className="status-pill">{analysis?.status || 'Pending'}</span>
      </div>

      <section className="analysis-kpi-grid">
        <article className="kpi-card">
          <p className="kpi-label">Clauses</p>
          <p className="kpi-value">{clauses.length}</p>
        </article>
        <article className="kpi-card">
          <p className="kpi-label">Risk findings</p>
          <p className="kpi-value">{risks.length}</p>
        </article>
        <article className="kpi-card">
          <p className="kpi-label">High priority</p>
          <p className="kpi-value">{highRiskCount}</p>
        </article>
      </section>

      <RiskScoreCard score={analysis?.riskScore} level={analysis?.riskLevel} />

      <div className="card summary-card">
        <h4>Summary</h4>
        <p className="body-copy">
          {analysis?.summary || 'No analysis available yet for this document.'}
        </p>
      </div>

      <div className="filter-row card compact">
        <div className="filter-control">
          <label htmlFor="severity-filter">Severity</label>
          <select
            id="severity-filter"
            value={severityFilter}
            onChange={(event) => setSeverityFilter(event.target.value)}
          >
            <option value="All">All</option>
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
            <option value="Critical">Critical</option>
          </select>
        </div>
        <div className="filter-control">
          <label htmlFor="clause-type-filter">Clause type</label>
          <select
            id="clause-type-filter"
            value={clauseTypeFilter}
            onChange={(event) => setClauseTypeFilter(event.target.value)}
          >
            {availableClauseTypes.map((type) => (
              <option key={type} value={type}>
                {type}
              </option>
            ))}
          </select>
        </div>
      </div>

      <h4 className="section-title sticky-section-title">Clauses</h4>
      <div className="stack">
        {filteredClauses.map((clause) => (
          <ClauseCard
            key={`${clause.title}-${clause.pageNumber}`}
            clause={clause}
            onJumpToPage={onJumpToPage}
          />
        ))}
        {filteredClauses.length === 0 ? (
          <div className="card empty-state">
            <p className="muted">No clauses match the selected filters.</p>
          </div>
        ) : null}
      </div>

      <h4 className="section-title sticky-section-title">Risk Findings</h4>
      <div className="stack">
        {filteredRisks.map((finding) => (
          <RiskFindingCard
            key={`${finding.riskTitle}-${finding.pageNumber}`}
            finding={finding}
            onJumpToPage={onJumpToPage}
          />
        ))}
        {filteredRisks.length === 0 ? (
          <div className="card empty-state">
            <p className="muted">No risk findings match the selected severity.</p>
          </div>
        ) : null}
      </div>
    </div>
  )
}
