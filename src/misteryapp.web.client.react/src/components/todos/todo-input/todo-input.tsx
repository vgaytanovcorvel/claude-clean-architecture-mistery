import { useState, type FormEvent } from 'react'
import styles from './todo-input.module.css'

interface TodoInputProps {
  onAdd: (title: string) => void
  disabled?: boolean
}

export function TodoInput({ onAdd, disabled }: TodoInputProps) {
  const [title, setTitle] = useState('')

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    const trimmed = title.trim()
    if (!trimmed) return
    onAdd(trimmed)
    setTitle('')
  }

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <input
        className={styles.input}
        type="text"
        value={title}
        onChange={e => setTitle(e.target.value)}
        placeholder="What needs to be done?"
        disabled={disabled}
        aria-label="New todo title"
      />
      <button
        className={styles.button}
        type="submit"
        disabled={disabled || !title.trim()}
        aria-label="Add todo"
      >
        <svg className={styles.icon} viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
          <path d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" />
        </svg>
      </button>
    </form>
  )
}
