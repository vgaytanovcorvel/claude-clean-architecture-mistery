import { Injectable, inject } from '@angular/core';
import { TodosMockApiService } from '../api/todos-mock-api.service';
import { Todo, CreateTodoRequest, UpdateTodoRequest } from '../../../domain/todo.model';
import { Result } from '../../../domain/result.model';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly api = inject(TodosMockApiService);

  getTodosForUser(userId: string): readonly Todo[] {
    return this.api.todoFindAllByUserId(userId);
  }

  createTodo(userId: string, req: CreateTodoRequest): Result<Todo> {
    const titleError = this.validateTitle(req.title);
    if (titleError) return Result.fail(titleError);

    const todo = this.api.todoCreate(userId, {
      ...req,
      title: req.title.trim(),
      description: req.description?.trim() ?? '',
    });
    return Result.ok(todo);
  }

  updateTodo(todoId: string, req: UpdateTodoRequest): Result<Todo> {
    if (req.title !== undefined) {
      const titleError = this.validateTitle(req.title);
      if (titleError) return Result.fail(titleError);
    }

    const updated = this.api.todoUpdate(todoId, {
      ...req,
      ...(req.title && { title: req.title.trim() }),
    });
    if (!updated) return Result.fail('Todo not found.');
    return Result.ok(updated);
  }

  deleteTodo(todoId: string): void {
    this.api.todoDelete(todoId);
  }

  toggleComplete(todoId: string, currentCompleted: boolean): Result<Todo> {
    const updated = this.api.todoUpdate(todoId, { completed: !currentCompleted });
    if (!updated) return Result.fail('Todo not found.');
    return Result.ok(updated);
  }

  private validateTitle(title: string): string | null {
    if (!title || title.trim().length === 0) return 'Title is required.';
    if (title.trim().length > 200) return 'Title must be 200 characters or fewer.';
    return null;
  }
}
