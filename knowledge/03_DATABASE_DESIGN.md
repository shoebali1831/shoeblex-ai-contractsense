# Database Design

Core tables:
- documents
- contract_chunks
- clauses
- risk_findings
- chat_messages

documents:
Stores uploaded PDF metadata.

contract_chunks:
Stores extracted text chunks and vector embeddings.

clauses:
Stores extracted legal clauses.

risk_findings:
Stores detected risky language.

chat_messages:
Stores user questions and AI answers.
