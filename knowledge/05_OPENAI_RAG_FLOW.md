# OpenAI and RAG Flow

Use OpenAI from backend only.

Embedding flow:
1. Extract contract text.
2. Split text into chunks.
3. Send each chunk to OpenAI embedding model.
4. Store returned vector in pgvector.

Chat flow:
1. User asks question.
2. Convert question to embedding.
3. Search pgvector for similar chunks.
4. Send retrieved chunks and question to OpenAI.
5. Return answer with source page references.

Important:
The AI must answer only from uploaded contract content.
If answer is not found, it must say it was not found.
