import { useQuery } from '@tanstack/react-query'
import { useServices } from '../../core/providers'

export function useTodoList(userId: number | null) {
  const { todoService } = useServices()

  return useQuery({
    queryKey: ['todos', userId],
    queryFn: () => todoService.getTodosByUser(userId!),
    enabled: userId !== null,
  })
}
