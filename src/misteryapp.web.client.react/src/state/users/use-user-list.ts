import { useQuery } from '@tanstack/react-query'
import { useServices } from '../../core/providers'

export function useUserList() {
  const { userService } = useServices()

  return useQuery({
    queryKey: ['users'],
    queryFn: () => userService.getUsers(),
  })
}
