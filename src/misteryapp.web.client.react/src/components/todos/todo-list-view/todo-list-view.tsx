import { AnimatePresence, motion } from 'framer-motion'
import styles from './todo-list-view.module.css'
import { Spinner } from '../../shared/spinner/spinner'
import { ErrorMessage } from '../../shared/error-message/error-message'
import { TodoCard } from '../todo-card/todo-card'
import { TodoInput } from '../todo-input/todo-input'
import type { TodoItem } from '../../../domain/models/todo-item'

interface TodoListViewProps {
  todos: readonly TodoItem[]
  isLoading: boolean
  error?: string
  isAddDisabled: boolean
  onAdd: (title: string) => void
  onToggle: (id: number) => void
  onDelete: (id: number) => void
  onUpdate: (id: number, title: string) => void
}

const itemVariants = {
  initial: { opacity: 0, y: 20, scale: 0.95 },
  animate: { opacity: 1, y: 0, scale: 1 },
  exit: { opacity: 0, scale: 0.9, transition: { duration: 0.2 } },
}

export function TodoListView({
  todos,
  isLoading,
  error,
  isAddDisabled,
  onAdd,
  onToggle,
  onDelete,
  onUpdate,
}: TodoListViewProps) {
  const pending = todos.filter(t => !t.isComplete)
  const completed = todos.filter(t => t.isComplete)

  return (
    <div className={styles.container}>
      <TodoInput onAdd={onAdd} disabled={isAddDisabled} />

      {isLoading && <Spinner />}
      {error && <ErrorMessage message={error} />}

      {!isLoading && !error && todos.length === 0 && (
        <motion.div
          className={styles.empty}
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4 }}
        >
          <p className={styles.emptyText}>No todos yet</p>
          <p className={styles.emptyHint}>Add one above to get started</p>
        </motion.div>
      )}

      {pending.length > 0 && (
        <ul className={styles.list}>
          <AnimatePresence mode="popLayout">
            {pending.map(todo => (
              <motion.div
                key={todo.id}
                layout
                variants={itemVariants}
                initial="initial"
                animate="animate"
                exit="exit"
                transition={{ type: 'spring', stiffness: 350, damping: 25 }}
              >
                <TodoCard
                  todo={todo}
                  onToggle={onToggle}
                  onDelete={onDelete}
                  onUpdate={onUpdate}
                />
              </motion.div>
            ))}
          </AnimatePresence>
        </ul>
      )}

      {completed.length > 0 && (
        <div className={styles.completedSection}>
          <h3 className={styles.sectionLabel}>
            Completed ({completed.length})
          </h3>
          <ul className={styles.list}>
            <AnimatePresence mode="popLayout">
              {completed.map(todo => (
                <motion.div
                  key={todo.id}
                  layout
                  variants={itemVariants}
                  initial="initial"
                  animate="animate"
                  exit="exit"
                  transition={{ type: 'spring', stiffness: 350, damping: 25 }}
                >
                  <TodoCard
                    todo={todo}
                    onToggle={onToggle}
                    onDelete={onDelete}
                    onUpdate={onUpdate}
                  />
                </motion.div>
              ))}
            </AnimatePresence>
          </ul>
        </div>
      )}
    </div>
  )
}
