import { Injectable, inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { firstValueFrom } from 'rxjs'
import { map } from 'rxjs/operators'
import { API_BASE_URL } from '../../app.config'
import { TodoItem } from '../../domain/models/todo-item'
import { ApiResponse } from '../../domain/models/api-response'
import { CreateTodoRequest, UpdateTodoRequest } from '../../domain/models/requests'

@Injectable({ providedIn: 'root' })
export class TodoApiService {
  private readonly http = inject(HttpClient)
  private readonly baseUrl = inject(API_BASE_URL)

  todoGetAll(username: string): Promise<readonly TodoItem[]> {
    return firstValueFrom(
      this.http.get<ApiResponse<TodoItem[]>>(`${this.baseUrl}/todos?username=${username}`).pipe(
        map(res => res.data ?? [])
      )
    )
  }

  todoCreate(username: string, data: CreateTodoRequest): Promise<TodoItem> {
    return firstValueFrom(
      this.http.post<ApiResponse<TodoItem>>(`${this.baseUrl}/todos?username=${username}`, data).pipe(
        map(res => res.data!)
      )
    )
  }

  todoUpdate(id: number, username: string, data: UpdateTodoRequest): Promise<TodoItem> {
    return firstValueFrom(
      this.http.put<ApiResponse<TodoItem>>(`${this.baseUrl}/todos/${id}?username=${username}`, data).pipe(
        map(res => res.data!)
      )
    )
  }

  todoDelete(id: number, username: string): Promise<void> {
    return firstValueFrom(
      this.http.delete<ApiResponse<void>>(`${this.baseUrl}/todos/${id}?username=${username}`).pipe(
        map(() => void 0)
      )
    )
  }

  todoToggle(id: number, username: string): Promise<TodoItem> {
    return firstValueFrom(
      this.http.patch<ApiResponse<TodoItem>>(`${this.baseUrl}/todos/${id}/toggle?username=${username}`, null).pipe(
        map(res => res.data!)
      )
    )
  }
}
