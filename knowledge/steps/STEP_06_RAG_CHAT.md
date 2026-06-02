# Step 6 — RAG Chat

Goal:
Allow users to ask questions about uploaded contracts.

Tasks:
1. Accept user question.
2. Convert question into embedding.
3. Search similar chunks using pgvector.
4. Send retrieved chunks to OpenAI.
5. Generate answer using only document context.
6. Return answer with page references.

API:
POST /api/chat/ask
