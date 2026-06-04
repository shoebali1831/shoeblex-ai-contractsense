import { useEffect, useMemo, useRef, useState } from 'react'
import { Document, Page, pdfjs } from 'react-pdf'
import 'react-pdf/dist/Page/AnnotationLayer.css'
import 'react-pdf/dist/Page/TextLayer.css'

pdfjs.GlobalWorkerOptions.workerSrc = new URL(
  'pdfjs-dist/build/pdf.worker.min.mjs',
  import.meta.url,
).toString()

type PdfViewerPanelProps = {
  pdfUrl?: string
  focusPage?: number
}

export function PdfViewerPanel({ pdfUrl, focusPage }: PdfViewerPanelProps) {
  const [numPages, setNumPages] = useState(0)
  const [pdfError, setPdfError] = useState('')
  const scrollRef = useRef<HTMLDivElement | null>(null)

  const pages = useMemo(
    () => Array.from({ length: numPages }, (_, index) => index + 1),
    [numPages],
  )

  useEffect(() => {
    if (!focusPage || !scrollRef.current) {
      return
    }

    const target = scrollRef.current.querySelector<HTMLElement>(
      `#pdf-page-${focusPage}`,
    )
    target?.scrollIntoView({ behavior: 'smooth', block: 'center' })
  }, [focusPage, numPages])

  return (
    <div className="panel viewer-panel">
      <div className="panel-header">
        <h3>Contract PDF</h3>
        <span className="status-pill">Read mode</span>
      </div>
      {pdfUrl ? (
        <div className="pdf-scroll" ref={scrollRef}>
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
              <div
                key={pageNumber}
                id={`pdf-page-${pageNumber}`}
                className={`pdf-page-wrap ${focusPage === pageNumber ? 'pdf-page-wrap-active' : ''}`}
              >
                <p className="pdf-page-label">Page {pageNumber}</p>
                <Page
                  pageNumber={pageNumber}
                  className="pdf-page"
                  renderTextLayer
                  renderAnnotationLayer
                />
              </div>
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
