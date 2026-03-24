import { motion } from 'framer-motion'
import styles from './todo-page.module.css'
import { useUserList } from '../../state/users/use-user-list'
import { useUserStore } from '../../state/users/user-store'
import { useTodoList } from '../../state/todos/use-todo-list'
import { useCreateTodo } from '../../state/todos/use-create-todo'
import { useToggleTodo } from '../../state/todos/use-toggle-todo'
import { useDeleteTodo } from '../../state/todos/use-delete-todo'
import { useUpdateTodo } from '../../state/todos/use-update-todo'
import { UserSelector } from '../../components/todos/user-selector/user-selector'
import { TodoListView } from '../../components/todos/todo-list-view/todo-list-view'
import { Spinner } from '../../components/shared/spinner/spinner'

export function TodoPage() {
  const { data: users = [], isLoading: usersLoading } = useUserList()
  const { selectedUserId, selectUser } = useUserStore()
  const { data: todos = [], isLoading: todosLoading, error: todosError } = useTodoList(selectedUserId)
  const { mutate: createTodo } = useCreateTodo()
  const { mutate: toggleTodo } = useToggleTodo()
  const { mutate: deleteTodo } = useDeleteTodo()
  const { mutate: updateTodo } = useUpdateTodo()

  function handleAdd(title: string) {
    if (selectedUserId === null) return
    createTodo({ title, userId: selectedUserId })
  }

  function handleUpdate(id: number, title: string) {
    updateTodo({ id, data: { title } })
  }

  return (
    <main className="o-page">
      <motion.div
        className="o-container"
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.6, ease: [0, 0, 0.2, 1] }}
      >
        <header className={styles.header}>
          <h1 className={styles.title}>Todos</h1>
          <p className={styles.subtitle}>Stay on track, one task at a time</p>
        </header>

        <div className={styles.panel}>
          {usersLoading ? (
            <Spinner />
          ) : (
            <>
              <UserSelector
                users={users}
                selectedUserId={selectedUserId}
                onSelect={selectUser}
              />

              {selectedUserId !== null && (
                <TodoListView
                  todos={todos}
                  isLoading={todosLoading}
                  error={todosError?.message}
                  isAddDisabled={selectedUserId === null}
                  onAdd={handleAdd}
                  onToggle={toggleTodo}
                  onDelete={deleteTodo}
                  onUpdate={handleUpdate}
                />
              )}

              {selectedUserId === null && !usersLoading && (
                <p className={styles.hint}>Select a user above to view their todos</p>
              )}
            </>
          )}
        </div>
      </motion.div>
    </main>
  )
}
