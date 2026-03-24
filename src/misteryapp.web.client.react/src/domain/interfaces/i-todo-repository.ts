import type { CreateTodoRequest } from '../models/create-todo-request'
import type { TodoItem } from '../models/todo-item'
import type { UpdateTodoRequest } from '../models/update-todo-request'

export interface ITodoRepository {
  todoFindByUserId(userId: number): Promise<readonly TodoItem[]>
  todoCreate(data: CreateTodoRequest): Promise<TodoItem>
  todoUpdate(id: number, data: UpdateTodoRequest): Promise<TodoItem>
  todoToggle(id: number): Promise<TodoItem>
  todoDelete(id: number): Promise<void>
}
