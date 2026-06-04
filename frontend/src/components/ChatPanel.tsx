import { useState } from 'react'
import type { AskResponse } from '../types'

type ChatPanelProps = {
  disabled: boolean
  isAsking: boolean
  onAsk: (question: string) => Promise<AskResponse>
}

type ChatItem = {
  question: string
  answer: string
  sourcePages: number[]
  disclaimer: string
}

export function ChatPanel({ disabled, isAsking, onAsk }: ChatPanelProps) {
  const [question, setQuestion] = useState('')
  const [history, setHistory] = useState<ChatItem[]>([])
  const [error, setError] = useState('')
  const suggestedQuestions = [
    'What are the payment terms?',
    'What are termination conditions?',
    'Are there liability limits?',
  ]

  async function submitQuestion() {
    if (!question.trim()) {
      return
    }

    try {
      setError('')
      const response = await onAsk(question.trim())
      setHistory((prev) => [
        ...prev,
        {
          question: question.trim(),
          answer: response.answer,
          sourcePages: response.sourcePages,
          disclaimer: response.disclaimer,
        },
      ])
      setQuestion('')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Chat failed.')
    }
  }

  async function handleSend(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    await submitQuestion()
  }

  return (
    <div className="panel">
      <div className="panel-header">
        <h3>Contract Chat</h3>
        <span className="status-pill">RAG grounded</span>
      </div>
      <p className="muted">
        Ask natural questions and get answers grounded in the uploaded contract.
      </p>
      <div className="suggestion-row">
        {suggestedQuestions.map((item) => (
          <button
            key={item}
            type="button"
            className="chip-button"
            disabled={disabled || isAsking}
            onClick={() => setQuestion(item)}
          >
            {item}
          </button>
        ))}
      </div>
      <form onSubmit={handleSend} className="chat-form">
        <input
          type="text"
          placeholder="Ask about clauses, obligations, or penalties..."
          value={question}
          onChange={(event) => setQuestion(event.target.value)}
          disabled={disabled || isAsking}
        />
        <button type="submit" disabled={disabled || isAsking}>
          {isAsking ? 'Sending...' : 'Ask'}
        </button>
      </form>
      {error ? <p className="error-banner">{error}</p> : null}
      <div className="chat-history">
        {history.length === 0 ? (
          <div className="card empty-state">
            <p className="muted">
              No questions yet. Start with one of the suggested prompts.
            </p>
          </div>
        ) : null}
        {history.map((item, index) => (
          <div key={`${item.question}-${index}`} className="card compact chat-item">
            <p className="chat-question chat-bubble chat-bubble-user">
              <strong>You</strong> {item.question}
            </p>
            <p className="chat-answer chat-bubble chat-bubble-ai">
              <strong>AI</strong> {item.answer}
            </p>
            <p className="muted meta-line">
              Sources: {item.sourcePages.length ? item.sourcePages.join(', ') : 'None'}
            </p>
            <p className="muted legal-note">{item.disclaimer}</p>
          </div>
        ))}
      </div>
    </div>
  )
}
