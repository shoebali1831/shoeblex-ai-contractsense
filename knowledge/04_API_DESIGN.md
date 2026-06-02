# API Design

Required APIs:

POST /api/documents/upload
Upload PDF contract.

GET /api/documents/{documentId}
Get document details.

GET /api/documents/{documentId}/file
Return PDF file for viewer.

GET /api/documents/{documentId}/analysis
Return summary, clauses, risks, and risk score.

POST /api/chat/ask
Ask question about uploaded contract using RAG.
