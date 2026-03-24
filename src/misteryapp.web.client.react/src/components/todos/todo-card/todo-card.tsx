import { useState, useRef, useEffect, type KeyboardEvent } from 'react'
import styles from './todo-card.module.css'
import type { TodoItem } from '../../../domain/models/todo-item'

interface TodoCardProps {
  todo: TodoItem
  onToggle: (id: number) => void
  onDelete: (id: number) => void
  onUpdate: (id: number, title: string) => void
}

export function TodoCard({ todo, onToggle, onDelete, onUpdate }: TodoCardProps) {
  const [isEditing, setIsEditing] = useState(false)
  const [editTitle, setEditTitle] = useState(todo.title)
  const inputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    if (isEditing) inputRef.current?.focus()
  }, [isEditing])

  function handleStartEdit() {
    if (todo.isComplete) return
    setEditTitle(todo.title)
    setIsEditing(true)
  }

  function handleCommitEdit() {
    const trimmed = editTitle.trim()
    if (trimmed && trimmed !== todo.title) {
      onUpdate(todo.id, trimmed)
    }
    setIsEditing(false)
  }

  function handleKeyDown(e: KeyboardEvent) {
    if (e.key === 'Enter') handleCommitEdit()
    if (e.key === 'Escape') setIsEditing(false)
  }

  return (
    <li className={`${styles.card} ${todo.isComplete ? styles.cardComplete : ''}`}>
      <button
        className={`${styles.checkbox} ${todo.isComplete ? styles.checkboxChecked : ''}`}
        onClick={() => onToggle(todo.id)}
        aria-label={todo.isComplete ? `Mark "${todo.title}" incomplete` : `Mark "${todo.title}" complete`}
      >
        {todo.isComplete && (
          <svg className={styles.checkIcon} viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
            <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
          </svg>
        )}
      </button>

      <div className={styles.body}>
        {isEditing ? (
          <input
            ref={inputRef}
            className={styles.editInput}
            value={editTitle}
            onChange={e => setEditTitle(e.target.value)}
            onBlur={handleCommitEdit}
            onKeyDown={handleKeyDown}
            aria-label="Edit todo title"
          />
        ) : (
          <span
            className={`${styles.title} ${todo.isComplete ? styles.titleComplete : ''}`}
            onDoubleClick={handleStartEdit}
          >
            {todo.title}
          </span>
        )}
        {todo.completedAtUtc && (
          <span className={styles.meta}>
            Completed {new Date(todo.completedAtUtc).toLocaleDateString()}
          </span>
        )}
      </div>

      <button
        className={styles.deleteBtn}
        onClick={() => onDelete(todo.id)}
        aria-label={`Delete "${todo.title}"`}
      >
        <svg className={styles.deleteIcon} viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
          <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
        </svg>
      </button>
    </li>
  )
}
