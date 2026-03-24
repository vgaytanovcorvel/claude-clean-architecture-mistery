import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useServices } from '../../core/providers'

export function useDeleteTodo() {
  const { todoService } = useServices()
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => todoService.deleteTodo(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
    },
  })
}
