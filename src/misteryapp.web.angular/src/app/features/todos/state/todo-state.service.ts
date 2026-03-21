import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { AuthStateService } from '../../../core/auth/auth-state.service';
import { TodoService } from '../services/todo.service';
import { Todo, CreateTodoRequest, UpdateTodoRequest, TodoFilter, PriorityFilter } from '../../../domain/todo.model';

@Injectable({ providedIn: 'root' })
export class TodoStateService {
  private readonly authState = inject(AuthStateService);
  private readonly todoService = inject(TodoService);

  // Raw todo list — refreshed synchronously after every mutation
  private readonly todos = signal<readonly Todo[]>([]);

  // UI filter state
  readonly statusFilter = signal<TodoFilter>('all');
  readonly priorityFilter = signal<PriorityFilter>('all');

  // Modal/form state
  readonly isFormOpen = signal(false);
  readonly editingTodo = signal<Todo | null>(null);

  // Derived — filtered todos
  readonly filteredTodos = computed(() => {
    const todos = [...this.todos()];
    const status = this.statusFilter();
    const priority = this.priorityFilter();

    return todos
      .filter((t) => {
        if (status === 'active') return !t.completed;
        if (status === 'completed') return t.completed;
        return true;
      })
      .filter((t) => priority === 'all' || t.priority === priority)
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
  });

  // Derived — counts
  readonly totalCount = computed(() => this.todos().length);
  readonly completedCount = computed(() => this.todos().filter((t) => t.completed).length);

  constructor() {
    // Reload todos whenever the logged-in user changes
    effect(() => {
      const userId = this.authState.session()?.userId;
      this.todos.set(userId ? this.todoService.getTodosForUser(userId) : []);
    });
  }

  private refreshTodos(): void {
    const userId = this.authState.session()?.userId;
    this.todos.set(userId ? this.todoService.getTodosForUser(userId) : []);
  }

  openCreateForm(): void {
    this.editingTodo.set(null);
    this.isFormOpen.set(true);
  }

  openEditForm(todo: Todo): void {
    this.editingTodo.set(todo);
    this.isFormOpen.set(true);
  }

  closeForm(): void {
    this.isFormOpen.set(false);
    this.editingTodo.set(null);
  }

  createTodo(req: CreateTodoRequest): string | null {
    const userId = this.authState.session()?.userId;
    if (!userId) return 'Not authenticated.';

    const result = this.todoService.createTodo(userId, req);
    if (!result.success) return result.error;

    this.refreshTodos();
    this.closeForm();
    return null;
  }

  updateTodo(todoId: string, req: UpdateTodoRequest): string | null {
    const result = this.todoService.updateTodo(todoId, req);
    if (!result.success) return result.error;

    this.refreshTodos();
    this.closeForm();
    return null;
  }

  deleteTodo(todoId: string): void {
    this.todoService.deleteTodo(todoId);
    this.refreshTodos();
  }

  toggleComplete(todo: Todo): void {
    this.todoService.toggleComplete(todo.id, todo.completed);
    this.refreshTodos();
  }

  setStatusFilter(filter: TodoFilter): void {
    this.statusFilter.set(filter);
  }

  setPriorityFilter(filter: PriorityFilter): void {
    this.priorityFilter.set(filter);
  }
}
