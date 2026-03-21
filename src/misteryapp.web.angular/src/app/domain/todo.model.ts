import { Priority } from './priority.model';

export interface Todo {
  id: string;
  userId: string;
  title: string;
  description: string;
  priority: Priority;
  completed: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTodoRequest {
  title: string;
  description: string;
  priority: Priority;
}

export interface UpdateTodoRequest {
  title?: string;
  description?: string;
  priority?: Priority;
  completed?: boolean;
}

export type TodoFilter = 'all' | 'active' | 'completed';
export type PriorityFilter = 'all' | Priority;
