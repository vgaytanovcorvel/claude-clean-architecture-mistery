import { Injectable, inject } from '@angular/core';
import { MockStorageService } from '../../../core/mock/mock-storage.service';
import { Todo, CreateTodoRequest, UpdateTodoRequest } from '../../../domain/todo.model';

@Injectable({ providedIn: 'root' })
export class TodosMockApiService {
  private readonly storage = inject(MockStorageService);

  todoFindAllByUserId(userId: string): readonly Todo[] {
    return this.storage.getTodos().filter((t) => t.userId === userId);
  }

  todoCreate(userId: string, req: CreateTodoRequest): Todo {
    const todos = this.storage.getTodos();
    const now = new Date().toISOString();
    const newTodo: Todo = {
      id: crypto.randomUUID(),
      userId,
      title: req.title,
      description: req.description,
      priority: req.priority,
      completed: false,
      createdAt: now,
      updatedAt: now,
    };
    this.storage.saveTodos([...todos, newTodo]);
    return newTodo;
  }

  todoUpdate(todoId: string, req: UpdateTodoRequest): Todo | null {
    const todos = this.storage.getTodos();
    const idx = todos.findIndex((t) => t.id === todoId);
    if (idx === -1) return null;
    const updated: Todo = {
      ...todos[idx],
      ...req,
      updatedAt: new Date().toISOString(),
    };
    const newTodos = [...todos.slice(0, idx), updated, ...todos.slice(idx + 1)];
    this.storage.saveTodos(newTodos);
    return updated;
  }

  todoDelete(todoId: string): void {
    const todos = this.storage.getTodos().filter((t) => t.id !== todoId);
    this.storage.saveTodos(todos);
  }
}
