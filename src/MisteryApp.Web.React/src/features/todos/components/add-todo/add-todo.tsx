import { useState, type FormEvent } from 'react'
import styles from './add-todo.module.css'

interface AddTodoProps {
  onAdd:    (text: string) => void
  disabled: boolean
}

export function AddTodo({ onAdd, disabled }: AddTodoProps) {
  const [text, setText] = useState('')

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    if (!text.trim()) return
    onAdd(text)
    setText('')
  }

  return (
    <form onSubmit={handleSubmit} className={styles.form}>
      <input
        className={styles.input}
        value={text}
        onChange={e => setText(e.target.value)}
        placeholder="Add a new todo..."
        disabled={disabled}
        aria-label="New todo text"
        maxLength={500}
      />
      <button
        type="submit"
        className={styles.addBtn}
        disabled={disabled || !text.trim()}
        aria-label="Add todo"
      >
        <svg viewBox="0 0 16 16" fill="none" aria-hidden="true">
          <path d="M8 2v12M2 8h12" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
        </svg>
      </button>
    </form>
  )
}
