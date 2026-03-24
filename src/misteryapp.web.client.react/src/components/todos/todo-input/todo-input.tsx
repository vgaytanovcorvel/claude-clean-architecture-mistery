import { useState, type FormEvent } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import styles from './todo-input.module.css'

interface TodoInputProps {
  onAdd: (title: string) => void
  disabled?: boolean
}

export function TodoInput({ onAdd, disabled }: TodoInputProps) {
  const [title, setTitle] = useState('')
  const hasText = title.trim().length > 0

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
        disabled={disabled || !hasText}
        aria-label="Add todo"
      >
        <AnimatePresence mode="wait" initial={false}>
          {hasText ? (
            <motion.svg
              key="send"
              className={styles.icon}
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
              initial={{ rotate: -90, opacity: 0, scale: 0.5 }}
              animate={{ rotate: 0, opacity: 1, scale: 1 }}
              exit={{ rotate: 90, opacity: 0, scale: 0.5 }}
              transition={{ type: 'spring', stiffness: 400, damping: 20 }}
            >
              <path d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z" />
            </motion.svg>
          ) : (
            <motion.svg
              key="plus"
              className={styles.icon}
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
              initial={{ rotate: 90, opacity: 0, scale: 0.5 }}
              animate={{ rotate: 0, opacity: 1, scale: 1 }}
              exit={{ rotate: -90, opacity: 0, scale: 0.5 }}
              transition={{ type: 'spring', stiffness: 400, damping: 20 }}
            >
              <path d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" />
            </motion.svg>
          )}
        </AnimatePresence>
      </button>
    </form>
  )
}
