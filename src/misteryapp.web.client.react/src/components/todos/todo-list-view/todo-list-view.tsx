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
        <div className={styles.empty}>
          <p className={styles.emptyText}>No todos yet</p>
          <p className={styles.emptyHint}>Add one above to get started</p>
        </div>
      )}

      {pending.length > 0 && (
        <ul className={styles.list}>
          {pending.map(todo => (
            <TodoCard
              key={todo.id}
              todo={todo}
              onToggle={onToggle}
              onDelete={onDelete}
              onUpdate={onUpdate}
            />
          ))}
        </ul>
      )}

      {completed.length > 0 && (
        <div className={styles.completedSection}>
          <h3 className={styles.sectionLabel}>
            Completed ({completed.length})
          </h3>
          <ul className={styles.list}>
            {completed.map(todo => (
              <TodoCard
                key={todo.id}
                todo={todo}
                onToggle={onToggle}
                onDelete={onDelete}
                onUpdate={onUpdate}
              />
            ))}
          </ul>
        </div>
      )}
    </div>
  )
}
