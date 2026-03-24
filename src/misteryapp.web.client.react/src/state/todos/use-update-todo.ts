import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useServices } from '../../core/providers'
import type { UpdateTodoRequest } from '../../domain/models/update-todo-request'

export function useUpdateTodo() {
  const { todoService } = useServices()
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateTodoRequest }) =>
      todoService.updateTodo(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
    },
  })
}
