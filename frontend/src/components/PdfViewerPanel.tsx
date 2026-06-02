import { useMemo, useState } from 'react'
import { Document, Page, pdfjs } from 'react-pdf'
import 'react-pdf/dist/Page/AnnotationLayer.css'
import 'react-pdf/dist/Page/TextLayer.css'

pdfjs.GlobalWorkerOptions.workerSrc = new URL(
  'pdfjs-dist/build/pdf.worker.min.mjs',
  import.meta.url,
).toString()

type PdfViewerPanelProps = {
  pdfUrl?: string
}

export function PdfViewerPanel({ pdfUrl }: PdfViewerPanelProps) {
  const [numPages, setNumPages] = useState(0)
  const [pdfError, setPdfError] = useState('')

  const pages = useMemo(
    () => Array.from({ length: numPages }, (_, index) => index + 1),
    [numPages],
  )

  return (
    <div className="panel viewer-panel">
      <div className="panel-header">
        <h3>Contract PDF</h3>
        <span className="status-pill">Read mode</span>
      </div>
      {pdfUrl ? (
        <div className="pdf-scroll">
          <Document
            file={pdfUrl}
            className="pdf-document"
            loading={<p className="muted">Loading PDF...</p>}
            onLoadSuccess={({ numPages: totalPages }) => {
              setNumPages(totalPages)
              setPdfError('')
            }}
            onLoadError={() => {
              setNumPages(0)
              setPdfError('Failed to render PDF. Please open it in a new tab.')
            }}
          >
            {pages.map((pageNumber) => (
              <Page
                key={pageNumber}
                pageNumber={pageNumber}
                className="pdf-page"
                renderTextLayer
                renderAnnotationLayer
              />
            ))}
          </Document>
          {pdfError ? <p className="error-banner">{pdfError}</p> : null}
        </div>
      ) : (
        <p className="muted">Upload a contract to preview it in the review pane.</p>
      )}
    </div>
  )
}
