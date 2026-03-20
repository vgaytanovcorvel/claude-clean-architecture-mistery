import type { InputHTMLAttributes } from 'react'
import styles from './input.module.css'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string
  error?: string
  hint?: string
}

export function Input({ label, error, hint, id, className, ...rest }: InputProps) {
  const inputId = id ?? label.toLowerCase().replace(/\s+/g, '-')
  return (
    <div className={styles.group}>
      <label className={styles.label} htmlFor={inputId}>{label}</label>
      <input
        id={inputId}
        className={[styles.input, error ? styles.inputError : '', className ?? ''].filter(Boolean).join(' ')}
        aria-describedby={error ? `${inputId}-error` : hint ? `${inputId}-hint` : undefined}
        aria-invalid={!!error}
        {...rest}
      />
      {error && <span id={`${inputId}-error`} className={styles.error} role="alert">{error}</span>}
      {hint && !error && <span id={`${inputId}-hint`} className={styles.hint}>{hint}</span>}
    </div>
  )
}
