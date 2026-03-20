import type { ITodoRepository } from '../domain/interfaces/i-todo-repository'
import type { Todo } from '../domain/models/todo'
import { Result } from '../domain/result'

export type TodoFilter = 'all' | 'active' | 'completed'

export class TodoService {
  constructor(private readonly todoRepo: ITodoRepository) {}

  async getTodos(userId: string, filter: TodoFilter = 'all'): Promise<Result<readonly Todo[]>> {
    const todos = await this.todoRepo.todoFindAll(userId)
    if (filter === 'all') return Result.ok(todos)
    const completed = filter === 'completed'
    return Result.ok(todos.filter(t => t.completed === completed))
  }

  async createTodo(userId: string, text: string): Promise<Result<Todo>> {
    if (!text.trim()) return Result.fail('Todo text cannot be empty')
    if (text.trim().length > 500) return Result.fail('Todo text is too long')
    const todo = await this.todoRepo.todoCreate(userId, text.trim())
    return Result.ok(todo)
  }

  async toggleTodo(userId: string, todo: Todo): Promise<Result<Todo>> {
    const updated = await this.todoRepo.todoUpdate(userId, todo.id, { completed: !todo.completed })
    return Result.ok(updated)
  }

  async updateTodoText(userId: string, todoId: string, text: string): Promise<Result<Todo>> {
    if (!text.trim()) return Result.fail('Todo text cannot be empty')
    if (text.trim().length > 500) return Result.fail('Todo text is too long')
    const updated = await this.todoRepo.todoUpdate(userId, todoId, { text: text.trim() })
    return Result.ok(updated)
  }

  async deleteTodo(userId: string, todoId: string): Promise<Result<void>> {
    await this.todoRepo.todoDelete(userId, todoId)
    return Result.ok(undefined)
  }
}
