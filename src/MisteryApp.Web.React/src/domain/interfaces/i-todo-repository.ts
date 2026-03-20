import type { Todo } from '../models/todo'

export interface ITodoRepository {
  todoFindAll(userId: string): Promise<readonly Todo[]>
  todoCreate(userId: string, text: string): Promise<Todo>
  todoUpdate(userId: string, id: string, changes: Partial<Pick<Todo, 'text' | 'completed'>>): Promise<Todo>
  todoDelete(userId: string, id: string): Promise<void>
}
