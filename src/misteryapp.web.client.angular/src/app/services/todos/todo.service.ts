import { Injectable, inject } from '@angular/core'
import { TodoApiService } from '../../repositories/todos/todo-api.service'
import { AuthService } from '../auth/auth.service'
import { TodoItem } from '../../domain/models/todo-item'
import { CreateTodoRequest, UpdateTodoRequest } from '../../domain/models/requests'

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly todoApi = inject(TodoApiService)
  private readonly authService = inject(AuthService)

  private getUsername(): string {
    return this.authService.getCurrentUser()!.username
  }

  getTodos(): Promise<readonly TodoItem[]> {
    return this.todoApi.todoGetAll(this.getUsername())
  }

  createTodo(data: CreateTodoRequest): Promise<TodoItem> {
    return this.todoApi.todoCreate(this.getUsername(), data)
  }

  updateTodo(id: number, data: UpdateTodoRequest): Promise<TodoItem> {
    return this.todoApi.todoUpdate(id, this.getUsername(), data)
  }

  deleteTodo(id: number): Promise<void> {
    return this.todoApi.todoDelete(id, this.getUsername())
  }

  toggleTodo(id: number): Promise<TodoItem> {
    return this.todoApi.todoToggle(id, this.getUsername())
  }
}
