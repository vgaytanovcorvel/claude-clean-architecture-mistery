import { useState, type KeyboardEvent } from 'react'
import type { Todo } from '../../../../domain/models/todo'
import styles from './todo-item.module.css'

interface TodoItemProps {
  todo: Todo
  onToggle:  (todo: Todo)            => void
  onDelete:  (todoId: string)        => void
  onRename:  (todoId: string, text: string) => void
}

export function TodoItem({ todo, onToggle, onDelete, onRename }: TodoItemProps) {
  const [editing,  setEditing]  = useState(false)
  const [editText, setEditText] = useState(todo.text)

  function commitEdit() {
    const trimmed = editText.trim()
    if (trimmed && trimmed !== todo.text) onRename(todo.id, trimmed)
    else setEditText(todo.text)
    setEditing(false)
  }

  function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
    if (e.key === 'Enter') commitEdit()
    if (e.key === 'Escape') { setEditText(todo.text); setEditing(false) }
  }

  return (
    <li className={[styles.item, todo.completed ? styles.completed : ''].filter(Boolean).join(' ')}>
      <button
        type="button"
        className={styles.checkbox}
        onClick={() => onToggle(todo)}
        aria-label={todo.completed ? 'Mark as active' : 'Mark as completed'}
        aria-pressed={todo.completed}
      >
        {todo.completed && (
          <svg className={styles.checkIcon} viewBox="0 0 12 10" fill="none" aria-hidden="true">
            <path d="M1 5l3.5 3.5L11 1" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        )}
      </button>

      {editing ? (
        <input
          className={styles.editInput}
          value={editText}
          onChange={e => setEditText(e.target.value)}
          onBlur={commitEdit}
          onKeyDown={handleKeyDown}
          autoFocus
          aria-label="Edit todo"
        />
      ) : (
        <span
          className={styles.text}
          onDoubleClick={() => { setEditing(true); setEditText(todo.text) }}
          title="Double-click to edit"
        >
          {todo.text}
        </span>
      )}

      <div className={styles.actions}>
        {!editing && (
          <button
            type="button"
            className={styles.editBtn}
            onClick={() => { setEditing(true); setEditText(todo.text) }}
            aria-label="Edit todo"
          >
            <svg viewBox="0 0 16 16" fill="none" aria-hidden="true">
              <path d="M11.5 2.5l2 2-8 8-2.5.5.5-2.5 8-8z" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </button>
        )}
        <button
          type="button"
          className={styles.deleteBtn}
          onClick={() => onDelete(todo.id)}
          aria-label="Delete todo"
        >
          <svg viewBox="0 0 16 16" fill="none" aria-hidden="true">
            <path d="M4 4l8 8M12 4l-8 8" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round"/>
          </svg>
        </button>
      </div>
    </li>
  )
}
