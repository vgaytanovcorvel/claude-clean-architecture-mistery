import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createElement, type ReactNode } from 'react'
import { useUserList } from './use-user-list'
import type { User } from '../../domain/models/user'
import type { IUserService } from '../../domain/interfaces/i-user-service'
import type { ITodoService } from '../../domain/interfaces/i-todo-service'
import type { IExampleService } from '../../domain/interfaces/i-example-service'

const mockUserService: IUserService = {
  getUsers: vi.fn(),
}

const mockTodoService: ITodoService = {
  getTodosByUser: vi.fn(),
  createTodo: vi.fn(),
  updateTodo: vi.fn(),
  toggleComplete: vi.fn(),
  deleteTodo: vi.fn(),
}

const mockExampleService: IExampleService = {
  getGreeting: vi.fn(),
}

vi.mock('../../core/providers', () => ({
  useServices: () => ({
    userService: mockUserService,
    todoService: mockTodoService,
    exampleService: mockExampleService,
  }),
}))

function createWrapper() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  })
  return function Wrapper({ children }: { children: ReactNode }) {
    return createElement(QueryClientProvider, { client: queryClient }, children)
  }
}

describe('useUserList', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should return users when the query succeeds', async () => {
    // Arrange
    const expectedUsers: readonly User[] = [
      { id: 1, name: 'Alice', avatarUrl: 'https://example.com/alice.png' },
      { id: 2, name: 'Bob', avatarUrl: 'https://example.com/bob.png' },
    ]
    vi.mocked(mockUserService.getUsers).mockResolvedValue(expectedUsers)

    // Act
    const { result } = renderHook(() => useUserList(), { wrapper: createWrapper() })

    // Assert
    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toEqual(expectedUsers)
    expect(mockUserService.getUsers).toHaveBeenCalledOnce()
  })

  it('should return error when the query fails', async () => {
    // Arrange
    vi.mocked(mockUserService.getUsers).mockRejectedValue(new Error('network error'))

    // Act
    const { result } = renderHook(() => useUserList(), { wrapper: createWrapper() })

    // Assert
    await waitFor(() => expect(result.current.isError).toBe(true))
    expect(result.current.error?.message).toBe('network error')
  })
})
