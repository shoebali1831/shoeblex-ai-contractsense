import { useEffect, useMemo, useState } from 'react'
import {
  Link,
  Navigate,
  Route,
  Routes,
  useNavigate,
  useParams,
} from 'react-router-dom'
import './App.css'
import {
  askQuestion,
  getDocumentAnalysis,
  getDocumentPdfUrl,
  uploadDocument,
} from './api/contractsenseApi'
import { AnalysisPanel } from './components/AnalysisPanel'
import { ChatPanel } from './components/ChatPanel'
import { LoadingSpinner } from './components/LoadingSpinner'
import { PdfViewerPanel } from './components/PdfViewerPanel'
import { UploadBox } from './components/UploadBox'
import type { DocumentAnalysis } from './types'

const audienceSegments = [
  'Business operations teams',
  'Finance teams',
  'In-house legal teams',
]

const workflowSteps = [
  {
    step: 'Step 1',
    title: 'Upload your contract',
    description:
      'Drop in a PDF contract and let the system process clauses, obligations, and risk signals in minutes.',
  },
  {
    step: 'Step 2',
    title: 'Review informational insights',
    description:
      'Get issue-by-issue findings with risk levels, plain-English explanations, and clause-focused context.',
  },
  {
    step: 'Step 3',
    title: 'Make informed decisions',
    description:
      'Use AI-supported insights and chat to improve terms, decide faster, and move negotiations forward.',
  },
]

function App() {
  return (
    <Routes>
      <Route path="/" element={<UploadRoute />} />
      <Route path="/review/:documentId" element={<ReviewRoute />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

function UploadRoute() {
  const navigate = useNavigate()
  const [isUploading, setIsUploading] = useState(false)
  const [error, setError] = useState('')

  async function handleUpload(file: File) {
    setIsUploading(true)
    setError('')
    try {
      const response = await uploadDocument(file)
      navigate(`/review/${response.documentId}`)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Upload failed')
    } finally {
      setIsUploading(false)
    }
  }

  return (
    <main className="landing-page">
      <section className="hero-shell">
        <div className="hero-content">
          <p className="eyebrow">Shoeblex AI ContractSense</p>
          <h1 className="hero-title">AI Legal Document Analyzer</h1>
          <p className="hero-subtitle">
            Upload a contract PDF and review summary, risk score, clauses, risk
            findings, and chat insights.
          </p>
          <div className="hero-actions">
            <a className="hero-cta" href="#upload-zone">
              Upload Contract
            </a>
          </div>
        </div>
        <div className="hero-points card">
          <h3>MVP Includes</h3>
          <ul>
            <li>PDF upload and extraction</li>
            <li>Risk score and analysis panel</li>
            <li>RAG chat with source references</li>
          </ul>
        </div>
      </section>

      <section className="audience-section">
        <p className="section-kicker">Who It Helps</p>
        <h2>Built for cross-functional deal teams</h2>
        <div className="audience-grid">
          {audienceSegments.map((segment) => (
            <div key={segment} className="audience-chip">
              {segment}
            </div>
          ))}
        </div>
      </section>

      <section className="section-shell">
        <p className="section-kicker">Workflow</p>
        <h2>How the review flow works</h2>
        <p className="muted">
          Organized from upload to decision, so every stakeholder can understand risk,
          language quality, and next actions quickly.
        </p>
      </section>

      <section className="workflow-grid">
        {workflowSteps.map((item) => (
          <article key={item.step} className="card workflow-card">
            <p className="eyebrow">{item.step}</p>
            <h3>{item.title}</h3>
            <p className="body-copy">{item.description}</p>
          </article>
        ))}
      </section>

      <section id="upload-zone" className="upload-shell">
        <UploadBox onUpload={handleUpload} isUploading={isUploading} />
        {isUploading ? (
          <LoadingSpinner label="Analyzing contract and generating AI insights..." />
        ) : null}
        {error ? <p className="error-banner">{error}</p> : null}
      </section>

      <p className="disclaimer-footnote">
        This AI analysis is for informational purposes only and is not legal advice.
      </p>
    </main>
  )
}

function ReviewRoute() {
  const { documentId = '' } = useParams()
  const [analysis, setAnalysis] = useState<DocumentAnalysis>()
  const [isLoading, setIsLoading] = useState(true)
  const [isAsking, setIsAsking] = useState(false)
  const [error, setError] = useState('')

  const pdfUrl = useMemo(
    () => (documentId ? getDocumentPdfUrl(documentId) : undefined),
    [documentId],
  )

  useEffect(() => {
    if (!documentId) {
      setIsLoading(false)
      return
    }

    void (async () => {
      try {
        const response = await getDocumentAnalysis(documentId)
        setAnalysis(response)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load analysis')
      } finally {
        setIsLoading(false)
      }
    })()
  }, [documentId])

  async function handleAsk(question: string) {
    if (!documentId) {
      throw new Error('Missing document id.')
    }

    setIsAsking(true)
    try {
      return await askQuestion(documentId, question)
    } finally {
      setIsAsking(false)
    }
  }

  if (!documentId) {
    return <Navigate to="/" replace />
  }

  return (
    <main className="review-page">
      <header className="review-header">
        <div>
          <p className="eyebrow">Contract Review Workspace</p>
          <h2 className="review-title">Document Intelligence Console</h2>
        </div>
        <div className="header-actions">
          <Link className="ghost-link" to="/">
            Upload another file
          </Link>
          <a className="ghost-link" href={pdfUrl} target="_blank" rel="noreferrer">
            Open PDF
          </a>
        </div>
      </header>

      {isLoading ? (
        <LoadingSpinner label="Loading analysis, clauses, and risk findings..." />
      ) : null}
      {error ? <p className="error-banner">{error}</p> : null}

      <section className="review-layout">
        <section className="left-pane">
          <PdfViewerPanel pdfUrl={pdfUrl} />
        </section>
        <section className="right-pane">
          <AnalysisPanel analysis={analysis} />
          <ChatPanel
            disabled={!analysis}
            isAsking={isAsking}
            onAsk={handleAsk}
          />
        </section>
      </section>
    </main>
  )
}

export default App
