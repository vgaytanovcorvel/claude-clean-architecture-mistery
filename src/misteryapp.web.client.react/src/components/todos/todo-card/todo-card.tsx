import { useState, useRef, useEffect, type KeyboardEvent, type MouseEvent } from 'react'
import confetti from 'canvas-confetti'
import styles from './todo-card.module.css'
import type { TodoItem } from '../../../domain/models/todo-item'

interface TodoCardProps {
  todo: TodoItem
  onToggle: (id: number) => void
  onDelete: (id: number) => void
  onUpdate: (id: number, title: string) => void
}

function fireConfetti(e: MouseEvent) {
  const rect = (e.target as HTMLElement).getBoundingClientRect()
  const x = (rect.left + rect.width / 2) / window.innerWidth
  const y = (rect.top + rect.height / 2) / window.innerHeight

  confetti({
    particleCount: 40,
    spread: 60,
    startVelocity: 20,
    gravity: 0.8,
    origin: { x, y },
    colors: ['#8b5cf6', '#d946ef', '#e879f9', '#22d3ee', '#a78bfa'],
    ticks: 60,
    scalar: 0.8,
  })
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

  function handleToggle(e: MouseEvent) {
    if (!todo.isComplete) {
      fireConfetti(e)
    }
    onToggle(todo.id)
  }

  return (
    <li className={`${styles.card} ${todo.isComplete ? styles.cardComplete : ''}`}>
      <button
        className={`${styles.checkbox} ${todo.isComplete ? styles.checkboxChecked : ''}`}
        onClick={handleToggle}
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

      <div className={styles.actions}>
        {!todo.isComplete && !isEditing && (
          <button
            className={styles.actionBtn}
            onClick={handleStartEdit}
            aria-label={`Edit "${todo.title}"`}
          >
            <svg className={styles.actionIcon} viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
              <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
            </svg>
          </button>
        )}
        <button
          className={styles.actionBtn}
          onClick={() => onDelete(todo.id)}
          aria-label={`Delete "${todo.title}"`}
        >
          <svg className={styles.actionIcon} viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
            <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
          </svg>
        </button>
      </div>
    </li>
  )
}
