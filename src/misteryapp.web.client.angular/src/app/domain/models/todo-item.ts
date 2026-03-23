export interface TodoItem {
  todoItemId: number
  userId: number
  title: string
  description: string | null
  isCompleted: boolean
  createdAt: string
  completedAt: string | null
}
