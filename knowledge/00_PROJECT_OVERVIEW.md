# Shoeblex AI ContractSense

Shoeblex AI ContractSense is an AI Legal Document Analyzer.

The user uploads a contract PDF. The backend extracts text, splits the text into chunks, generates OpenAI embeddings, stores vectors in PostgreSQL using pgvector, extracts clauses, detects risky language, calculates a risk score, and allows the user to ask questions using RAG.

MVP features:
- PDF upload
- PDF text extraction
- chunking
- OpenAI embeddings
- PostgreSQL + pgvector
- RAG chat
- clause extraction
- risk scoring
- split-pane PDF viewer

Skip for MVP:
- payments
- admin panel
- OCR
- multi-tenant SaaS
- exact PDF text highlighting
- advanced legal validation
