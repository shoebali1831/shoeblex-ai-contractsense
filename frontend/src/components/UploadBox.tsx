import { useState } from 'react'

type UploadBoxProps = {
  onUpload: (file: File) => Promise<void>
  isUploading: boolean
}

export function UploadBox({ onUpload, isUploading }: UploadBoxProps) {
  const [error, setError] = useState<string>('')
  const [selectedFileName, setSelectedFileName] = useState<string>('')

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const form = event.currentTarget
    const input = form.elements.namedItem('pdfFile') as HTMLInputElement | null
    const file = input?.files?.[0]

    if (!file) {
      setError('Please choose a PDF file.')
      return
    }

    if (file.type !== 'application/pdf' && !file.name.endsWith('.pdf')) {
      setError('Only PDF files are accepted.')
      return
    }

    setError('')
    await onUpload(file)
    form.reset()
    setSelectedFileName('')
  }

  return (
    <div className="card upload-card">
      <h2>Upload Contract PDF</h2>
      <p className="muted">
        Supported: standard text-based PDF contracts. We process your file and return
        risks, clauses, and chat-ready context.
      </p>
      <form onSubmit={handleSubmit} className="upload-form">
        <label className="file-picker">
          <input
            type="file"
            name="pdfFile"
            accept=".pdf,application/pdf"
            onChange={(event) =>
              setSelectedFileName(event.currentTarget.files?.[0]?.name || '')
            }
          />
          <span>{selectedFileName || 'Choose PDF file'}</span>
        </label>
        <button type="submit" disabled={isUploading}>
          {isUploading ? 'Processing...' : 'Upload & Analyze'}
        </button>
      </form>
      {error ? <p className="error-banner">{error}</p> : null}
    </div>
  )
}
