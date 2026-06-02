import type { AskResponse, DocumentAnalysis, UploadResponse } from '../types'

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL?.trim() || 'http://localhost:5224'

async function parseJson<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const errorPayload = (await response.json().catch(() => null)) as
      | { message?: string }
      | null
    throw new Error(errorPayload?.message || `Request failed: ${response.status}`)
  }

  return (await response.json()) as T
}

export async function uploadDocument(file: File): Promise<UploadResponse> {
  const formData = new FormData()
  formData.append('file', file)

  let response: Response
  try {
    response = await fetch(`${API_BASE_URL}/api/documents/upload`, {
      method: 'POST',
      body: formData,
    })
  } catch {
    throw new Error(
      'Could not reach backend API. Ensure backend is running on http://localhost:5224.',
    )
  }

  return parseJson<UploadResponse>(response)
}

export async function getDocumentAnalysis(
  documentId: string,
): Promise<DocumentAnalysis> {
  const response = await fetch(`${API_BASE_URL}/api/documents/${documentId}/analysis`)
  return parseJson<DocumentAnalysis>(response)
}

export function getDocumentPdfUrl(documentId: string): string {
  return `${API_BASE_URL}/api/documents/${documentId}/file`
}

export async function askQuestion(
  documentId: string,
  question: string,
): Promise<AskResponse> {
  const response = await fetch(`${API_BASE_URL}/api/chat/ask`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ documentId, question }),
  })

  return parseJson<AskResponse>(response)
}
