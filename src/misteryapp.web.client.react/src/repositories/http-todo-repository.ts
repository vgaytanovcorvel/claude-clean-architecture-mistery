import type { ITodoRepository } from '../domain/interfaces/i-todo-repository'
import type { CreateTodoRequest } from '../domain/models/create-todo-request'
import type { TodoItem } from '../domain/models/todo-item'
import type { UpdateTodoRequest } from '../domain/models/update-todo-request'
import type { ApiClient } from '../core/api-client'

interface ApiResponse<T> {
  success: boolean
  data: T
  error: string | null
  statusCode: number
}

export class HttpTodoRepository implements ITodoRepository {
  constructor(private readonly http: ApiClient) {}

  async todoFindByUserId(userId: number): Promise<readonly TodoItem[]> {
    const response = await this.http.get<ApiResponse<TodoItem[]>>(`/api/todos?userId=${userId}`)
    return response.data
  }

  async todoCreate(data: CreateTodoRequest): Promise<TodoItem> {
    const response = await this.http.post<ApiResponse<TodoItem>>('/api/todos', data)
    return response.data
  }

  async todoUpdate(id: number, data: UpdateTodoRequest): Promise<TodoItem> {
    const response = await this.http.put<ApiResponse<TodoItem>>(`/api/todos/${id}`, data)
    return response.data
  }

  async todoToggle(id: number): Promise<TodoItem> {
    const response = await this.http.put<ApiResponse<TodoItem>>(`/api/todos/${id}/toggle`, {})
    return response.data
  }

  async todoDelete(id: number): Promise<void> {
    await this.http.delete(`/api/todos/${id}`)
  }
}
