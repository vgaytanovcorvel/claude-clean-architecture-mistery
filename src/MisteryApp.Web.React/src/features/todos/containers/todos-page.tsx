import { useState, useEffect, useCallback } from 'react'
import { useAuth } from '../../../core/auth-context'
import { useServices } from '../../../core/services-context'
import type { Todo } from '../../../domain/models/todo'
import type { TodoFilter } from '../../../services/todo-service'
import { Layout } from '../../../shared/components/layout/layout'
import { GlassCard } from '../../../shared/components/glass-card/glass-card'
import { TodoItem } from '../components/todo-item/todo-item'
import { AddTodo } from '../components/add-todo/add-todo'
import styles from './todos-page.module.css'

const FILTERS: { label: string; value: TodoFilter }[] = [
  { label: 'All',       value: 'all'       },
  { label: 'Active',    value: 'active'    },
  { label: 'Completed', value: 'completed' },
]

export function TodosPage() {
  const { currentUser } = useAuth()
  const { todoService } = useServices()

  const [todos,   setTodos]   = useState<Todo[]>([])
  const [filter,  setFilter]  = useState<TodoFilter>('all')
  const [loading, setLoading] = useState(true)
  const [adding,  setAdding]  = useState(false)

  const loadTodos = useCallback(async () => {
    if (!currentUser) return
    const result = await todoService.getTodos(currentUser.id, filter)
    if (result.success) setTodos([...result.value])
    setLoading(false)
  }, [currentUser, todoService, filter])

  useEffect(() => { void loadTodos() }, [loadTodos])

  async function handleAdd(text: string) {
    if (!currentUser) return
    setAdding(true)
    const result = await todoService.createTodo(currentUser.id, text)
    if (result.success) await loadTodos()
    setAdding(false)
  }

  async function handleToggle(todo: Todo) {
    if (!currentUser) return
    const result = await todoService.toggleTodo(currentUser.id, todo)
    if (result.success) await loadTodos()
  }

  async function handleDelete(todoId: string) {
    if (!currentUser) return
    await todoService.deleteTodo(currentUser.id, todoId)
    await loadTodos()
  }

  async function handleRename(todoId: string, text: string) {
    if (!currentUser) return
    await todoService.updateTodoText(currentUser.id, todoId, text)
    await loadTodos()
  }

  const activeCount = todos.filter(t => !t.completed).length

  return (
    <Layout>
      <div className={styles.page}>
        <header className={styles.pageHeader}>
          <h1 className={styles.pageTitle}>My Todos</h1>
          {!loading && (
            <span className={styles.badge}>
              {activeCount} {activeCount === 1 ? 'task' : 'tasks'} remaining
            </span>
          )}
        </header>

        <GlassCard className={styles.card}>
          <AddTodo onAdd={handleAdd} disabled={adding || loading} />

          <div className={styles.filterBar}>
            {FILTERS.map(f => (
              <button
                key={f.value}
                type="button"
                className={[styles.filterBtn, filter === f.value ? styles.filterBtnActive : ''].filter(Boolean).join(' ')}
                onClick={() => setFilter(f.value)}
              >
                {f.label}
              </button>
            ))}
          </div>

          {loading ? (
            <div className={styles.emptyState}>
              <div className={styles.spinner} aria-label="Loading..." />
            </div>
          ) : todos.length === 0 ? (
            <div className={styles.emptyState}>
              <p className={styles.emptyIcon} aria-hidden="true">
                {filter === 'completed' ? '✓' : '○'}
              </p>
              <p className={styles.emptyText}>
                {filter === 'all'       ? 'No todos yet. Add one above!' :
                 filter === 'active'    ? 'No active todos. Great work!' :
                                          'Nothing completed yet.'}
              </p>
            </div>
          ) : (
            <ul className={styles.list}>
              {todos.map(todo => (
                <TodoItem
                  key={todo.id}
                  todo={todo}
                  onToggle={handleToggle}
                  onDelete={handleDelete}
                  onRename={handleRename}
                />
              ))}
            </ul>
          )}
        </GlassCard>
      </div>
    </Layout>
  )
}
