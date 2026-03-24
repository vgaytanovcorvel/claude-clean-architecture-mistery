import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useServices } from '../../core/providers'
import type { CreateTodoRequest } from '../../domain/models/create-todo-request'

export function useCreateTodo() {
  const { todoService } = useServices()
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateTodoRequest) => todoService.createTodo(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
    },
  })
}
