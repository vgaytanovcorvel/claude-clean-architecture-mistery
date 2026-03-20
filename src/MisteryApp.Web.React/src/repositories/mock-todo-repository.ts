import type { ITodoRepository } from '../domain/interfaces/i-todo-repository'
import type { Todo } from '../domain/models/todo'
import { NotFoundException } from '../domain/errors'

function storageKey(userId: string): string {
  return `mistery_todos_${userId}`
}

function loadTodos(userId: string): Todo[] {
  try {
    return JSON.parse(localStorage.getItem(storageKey(userId)) ?? '[]') as Todo[]
  } catch {
    return []
  }
}

function saveTodos(userId: string, todos: Todo[]): void {
  localStorage.setItem(storageKey(userId), JSON.stringify(todos))
}

export class MockTodoRepository implements ITodoRepository {
  async todoFindAll(userId: string): Promise<readonly Todo[]> {
    return loadTodos(userId)
  }

  async todoCreate(userId: string, text: string): Promise<Todo> {
    const todos = loadTodos(userId)
    const newTodo: Todo = {
      id: crypto.randomUUID(),
      userId,
      text,
      completed: false,
      createdAt: new Date().toISOString(),
    }
    saveTodos(userId, [...todos, newTodo])
    return newTodo
  }

  async todoUpdate(
    userId: string,
    id: string,
    changes: Partial<Pick<Todo, 'text' | 'completed'>>,
  ): Promise<Todo> {
    const todos = loadTodos(userId)
    const idx = todos.findIndex(t => t.id === id)
    if (idx === -1) throw new NotFoundException(`Todo not found (TodoId: ${id})`)
    const updated = { ...todos[idx], ...changes }
    saveTodos(userId, [...todos.slice(0, idx), updated, ...todos.slice(idx + 1)])
    return updated
  }

  async todoDelete(userId: string, id: string): Promise<void> {
    const todos = loadTodos(userId)
    const filtered = todos.filter(t => t.id !== id)
    saveTodos(userId, filtered)
  }
}
