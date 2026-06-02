CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE IF NOT EXISTS documents (
    id UUID PRIMARY KEY,
    original_file_name VARCHAR(255) NOT NULL,
    stored_file_path VARCHAR(512) NOT NULL,
    status VARCHAR(64) NOT NULL,
    risk_score INTEGER NULL,
    risk_level VARCHAR(32) NULL,
    extracted_text TEXT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS contract_chunks (
    id UUID PRIMARY KEY,
    document_id UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    page_number INTEGER NOT NULL,
    chunk_index INTEGER NOT NULL,
    content TEXT NOT NULL,
    embedding vector(1536) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS clauses (
    id UUID PRIMARY KEY,
    document_id UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    clause_type VARCHAR(128) NOT NULL,
    title VARCHAR(256) NOT NULL,
    summary TEXT NOT NULL,
    risk_level VARCHAR(32) NOT NULL,
    risk_reason TEXT NOT NULL,
    page_number INTEGER NOT NULL,
    source_text TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS risk_findings (
    id UUID PRIMARY KEY,
    document_id UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    risk_title VARCHAR(256) NOT NULL,
    severity VARCHAR(32) NOT NULL,
    explanation TEXT NOT NULL,
    recommendation TEXT NOT NULL,
    page_number INTEGER NOT NULL,
    source_text TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS chat_messages (
    id UUID PRIMARY KEY,
    document_id UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    question TEXT NOT NULL,
    answer TEXT NOT NULL,
    source_pages TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
