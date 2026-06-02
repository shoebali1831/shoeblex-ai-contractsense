# Shoeblex AI ContractSense - Project Documentation

## 1) Project Overview

Shoeblex AI ContractSense is an AI-powered legal document analyzer.

It lets users upload a contract PDF and then:
- extracts contract text page-by-page
- chunks text and generates embeddings
- stores chunks in PostgreSQL with `pgvector`
- retrieves relevant context using RAG for Q&A
- extracts clauses and risk findings
- calculates a risk score
- displays everything in a split-pane UI (PDF + analysis/chat)

Legal notice used by the app:
`This AI analysis is for informational purposes only and is not legal advice.`

---

## 2) Tech Stack

- Frontend: React + TypeScript + Vite
- Backend: ASP.NET Core Web API (.NET 10)
- Database: PostgreSQL + `pgvector`
- AI: OpenAI-compatible API (configured via backend only)

---

## 3) Repository Structure

- `backend/ContractSense.Api` - API, services, models, controllers
- `frontend` - React app
- `database/init.sql` - DB bootstrap schema
- `tests/ContractSense.Api.Tests` - backend unit tests
- `knowledge` - project knowledge and implementation stories

---

## 4) Implemented MVP Scope

Implemented (as defined by knowledge stories):
- Backend setup and health endpoint
- Database setup with core tables and vector support
- PDF upload endpoint with PDF validation
- Text extraction from uploaded PDF
- Chunking with overlap + embeddings
- Vector retrieval (document-scoped)
- RAG chat endpoint
- Clause extraction + risk findings + risk scoring
- Analysis endpoint
- Frontend upload page
- Split-pane review page with PDF + analysis + chat
- Demo sample contract and README run instructions

---

## 5) Architecture and Flow

1. User uploads PDF (`POST /api/documents/upload`)
2. Backend stores file and document metadata
3. PDF text is extracted per page
4. Text is chunked and embeddings are generated
5. Chunks + vectors are stored in DB (`contract_chunks`)
6. Clauses and risks are generated and stored
7. Risk score is calculated and persisted on `documents`
8. Frontend review page calls analysis endpoint
9. Chat endpoint embeds question, retrieves top chunks by vector similarity, and answers from retrieved context

---

## 6) Database Schema

From `database/init.sql`:
- `documents`
- `contract_chunks` (with `embedding vector(1536)`)
- `clauses`
- `risk_findings`
- `chat_messages`

`pgvector` is enabled with:
```sql
CREATE EXTENSION IF NOT EXISTS vector;
```

---

## 7) API Endpoints

### Health
- `GET /api/health`

### Documents
- `POST /api/documents/upload`
- `GET /api/documents/{documentId}`
- `GET /api/documents/{documentId}/file`
- `GET /api/documents/{documentId}/analysis`

### Chat
- `POST /api/chat/ask`

---

## 8) Configuration

Backend config keys:
- `ConnectionStrings:DefaultConnection`
- `OpenAI:ApiKey`
- `OpenAI:BaseUrl`
- `OpenAI:ChatModel`
- `OpenAI:EmbeddingModel`
- `OpenAI:HttpReferer` (optional provider header)
- `OpenAI:XTitle` (optional provider header)

Security rule:
- AI key must stay backend-only (never frontend).

---

## 9) How to Run Locally

## 9.1 Database
```bash
psql -h localhost -U postgres -d contractsense -f database/init.sql
```

## 9.2 Backend
```bash
dotnet restore backend/ContractSense.Api/ContractSense.Api.csproj
dotnet ef database update --project backend/ContractSense.Api/ContractSense.Api.csproj --startup-project backend/ContractSense.Api/ContractSense.Api.csproj
dotnet run --project backend/ContractSense.Api/ContractSense.Api.csproj
```

Health check:
```bash
curl http://localhost:5224/api/health
```

## 9.3 Frontend
```bash
npm install --prefix frontend
npm run --prefix frontend dev
```

---

## 10) Testing and CI

### Automated tests
- Backend tests are in `tests/ContractSense.Api.Tests`
- Example coverage includes:
  - risk scoring behavior
  - RAG question length validation

Run tests:
```bash
dotnet test tests/ContractSense.Api.Tests/ContractSense.Api.Tests.csproj
```

### CI
- GitHub Actions workflow: `.github/workflows/ci.yml`
- Runs:
  - .NET restore/build
  - backend tests
  - frontend install/build

---

## 11) Reliability and Safety Enhancements Implemented

- Standardized API error envelope (`code`, `message`, `traceId`)
- Global exception handler for consistent 500 responses
- Prompt-injection-aware RAG system prompt
- Answer consistency fix: no source pages when answer is "not found"
- Two-pass retrieval:
  1) semantic vector retrieval
  2) keyword fallback retrieval
- Value-extraction fallback for amount/rent-style questions
- Clause/risk JSON parsing cleanup and normalization

---

## 12) Frontend Features

- Upload contract page
- Split-pane review page
  - left: PDF viewer
  - right: summary, risk score, clauses, risk findings, chat
- Loading and error states
- Suggested chat prompts
- Legal disclaimer display

---

## 13) Known Limitations

- Not a substitute for legal counsel
- Accuracy depends on quality of extracted PDF text
- Scanned-image OCR is not included in MVP
- Multi-tenant auth/roles and advanced workflow approvals are not in MVP

---

## 14) Demo Script (Quick)

1. Start DB, backend, frontend
2. Upload `frontend/public/sample-contract.pdf`
3. Open review page
4. Show:
   - risk score
   - clauses/risks panel
   - chat answer with source pages
5. Ask:
   - "What are the payment terms?"
   - "What is the monthly rent?" (for rent contracts)
6. Highlight disclaimer and informational-use nature

---

## 15) Compliance Statement

This system provides legal information for contract review support and is not legal advice.
