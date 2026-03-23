export interface CreateTodoRequest {
  title: string
  description: string | null
}

export interface UpdateTodoRequest {
  title: string
  description: string | null
  isCompleted: boolean
}

export interface LoginRequest {
  username: string
  displayName: string
}
