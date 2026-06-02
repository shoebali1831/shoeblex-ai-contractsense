# Shoeblex AI ContractSense

Shoeblex AI ContractSense is a full-stack AI legal document analyzer. Users upload a contract PDF, the backend extracts and chunks text, generates embeddings, stores vectors in PostgreSQL + pgvector, and provides RAG-based answers with source pages.

## Stack

- Frontend: React + TypeScript (Vite)
- Backend: ASP.NET Core Web API
- Database: PostgreSQL + pgvector
- AI: OpenAI API (backend-only)

## Prerequisites

- .NET SDK 10+
- Node.js 20+
- PostgreSQL 15+ with `pgvector` extension
- OpenAI API key

## Environment

Update backend config in `backend/ContractSense.Api/appsettings.Development.json`:

- `ConnectionStrings:DefaultConnection`
- `OpenAI:ApiKey`

Or use user-secrets/environment variables in your local environment.

## Database Setup

Run the SQL file:

```bash
psql -h localhost -U postgres -d contractsense -f database/init.sql
```

## Run Backend

```bash
dotnet restore backend/ContractSense.Api/ContractSense.Api.csproj
dotnet run --project backend/ContractSense.Api/ContractSense.Api.csproj
```

Backend health check:

```bash
curl http://localhost:5224/api/health
```

## Run Frontend

```bash
npm install --prefix frontend
npm run --prefix frontend dev
```

Frontend opens on Vite default URL (usually `http://localhost:5173`).

## API Endpoints

- `POST /api/documents/upload`
- `GET /api/documents/{documentId}`
- `GET /api/documents/{documentId}/file`
- `GET /api/documents/{documentId}/analysis`
- `POST /api/chat/ask`

## Demo Questions

- What are the payment terms?
- Is there an automatic renewal clause?
- What does the contract say about termination?
- Are there liability limitations?

## Legal Disclaimer

This AI analysis is for informational purposes only and is not legal advice.
