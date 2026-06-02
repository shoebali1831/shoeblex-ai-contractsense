# Architecture

Frontend: React + TypeScript

Backend: ASP.NET Core Web API

Database: PostgreSQL + pgvector

AI: OpenAI API

Main flow:
User uploads PDF
-> Backend saves PDF
-> Backend extracts text page by page
-> Text is cleaned
-> Text is split into chunks
-> OpenAI creates embeddings
-> Chunks and vectors are stored in PostgreSQL pgvector
-> User asks question
-> Question is embedded
-> pgvector retrieves similar chunks
-> OpenAI answers using retrieved chunks only
-> Frontend displays answer with page references
