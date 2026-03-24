import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useServices } from '../../core/providers'

export function useToggleTodo() {
  const { todoService } = useServices()
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: number) => todoService.toggleComplete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
    },
  })
}
