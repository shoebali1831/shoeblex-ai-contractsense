export interface UploadResponse {
  documentId: string
  fileName: string
  status: string
  message: string
}

export interface DocumentAnalysis {
  documentId: string
  status: string
  riskScore?: number
  riskLevel?: string
  summary: string
  clauses: Clause[]
  risks: RiskFinding[]
}

export interface Clause {
  clauseType: string
  title: string
  summary: string
  riskLevel: string
  riskReason: string
  pageNumber: number
  sourceText: string
}

export interface RiskFinding {
  riskTitle: string
  severity: string
  explanation: string
  recommendation: string
  pageNumber: number
  sourceText: string
}

export interface AskResponse {
  answer: string
  sourcePages: number[]
  disclaimer: string
}
