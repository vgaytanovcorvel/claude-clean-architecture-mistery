import type { ITodoRepository } from '../domain/interfaces/i-todo-repository'
import type { ITodoService } from '../domain/interfaces/i-todo-service'
import type { CreateTodoRequest } from '../domain/models/create-todo-request'
import type { TodoItem } from '../domain/models/todo-item'
import type { UpdateTodoRequest } from '../domain/models/update-todo-request'

export class TodoService implements ITodoService {
  constructor(private readonly todoRepo: ITodoRepository) {}

  getTodosByUser(userId: number): Promise<readonly TodoItem[]> {
    return this.todoRepo.todoFindByUserId(userId)
  }

  createTodo(data: CreateTodoRequest): Promise<TodoItem> {
    return this.todoRepo.todoCreate(data)
  }

  updateTodo(id: number, data: UpdateTodoRequest): Promise<TodoItem> {
    return this.todoRepo.todoUpdate(id, data)
  }

  toggleComplete(id: number): Promise<TodoItem> {
    return this.todoRepo.todoToggle(id)
  }

  deleteTodo(id: number): Promise<void> {
    return this.todoRepo.todoDelete(id)
  }
}
