export interface TodoItem {
  id: number
  title: string
  isComplete: boolean
  userId: number
  createdAtUtc: string
  completedAtUtc: string | null
}
