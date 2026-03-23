import { Injectable, inject, signal, computed, resource } from '@angular/core'
import { TodoService } from '../../services/todos/todo.service'
import { AuthStateService } from '../auth/auth-state.service'
import { TodoItem } from '../../domain/models/todo-item'
import { CreateTodoRequest, UpdateTodoRequest } from '../../domain/models/requests'

export type TodoFilter = 'all' | 'active' | 'completed'

@Injectable({ providedIn: 'root' })
export class TodoStateService {
  private readonly todoService = inject(TodoService)
  private readonly authState = inject(AuthStateService)

  readonly filter = signal<TodoFilter>('all')

  readonly todos = resource({
    request: () => {
      const user = this.authState.currentUser()
      return user ? { userId: user.userId } : undefined
    },
    loader: () => this.todoService.getTodos(),
  })

  readonly filteredTodos = computed(() => {
    const items = this.todos.value() ?? []
    const currentFilter = this.filter()
    if (currentFilter === 'active') return items.filter(t => !t.isCompleted)
    if (currentFilter === 'completed') return items.filter(t => t.isCompleted)
    return items
  })

  async createTodo(request: CreateTodoRequest): Promise<void> {
    await this.todoService.createTodo(request)
    this.todos.reload()
  }

  async updateTodo(id: number, request: UpdateTodoRequest): Promise<void> {
    await this.todoService.updateTodo(id, request)
    this.todos.reload()
  }

  async deleteTodo(id: number): Promise<void> {
    await this.todoService.deleteTodo(id)
    this.todos.reload()
  }

  async toggleTodo(id: number): Promise<void> {
    await this.todoService.toggleTodo(id)
    this.todos.reload()
  }
}
