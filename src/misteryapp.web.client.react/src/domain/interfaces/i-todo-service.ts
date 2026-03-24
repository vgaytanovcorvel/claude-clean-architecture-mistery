import type { CreateTodoRequest } from '../models/create-todo-request'
import type { TodoItem } from '../models/todo-item'
import type { UpdateTodoRequest } from '../models/update-todo-request'

export interface ITodoService {
  getTodosByUser(userId: number): Promise<readonly TodoItem[]>
  createTodo(data: CreateTodoRequest): Promise<TodoItem>
  updateTodo(id: number, data: UpdateTodoRequest): Promise<TodoItem>
  toggleComplete(id: number): Promise<TodoItem>
  deleteTodo(id: number): Promise<void>
}
