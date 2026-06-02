type LoadingSpinnerProps = {
  label: string
}

export function LoadingSpinner({ label }: LoadingSpinnerProps) {
  return (
    <div className="loading-row" role="status" aria-live="polite">
      <span className="spinner" aria-hidden="true" />
      <span className="loading-label">{label}</span>
    </div>
  )
}
