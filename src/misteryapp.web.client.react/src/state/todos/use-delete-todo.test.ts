import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor, act } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createElement, type ReactNode } from 'react'
import { useDeleteTodo } from './use-delete-todo'
import type { ITodoService } from '../../domain/interfaces/i-todo-service'
import type { IUserService } from '../../domain/interfaces/i-user-service'
import type { IExampleService } from '../../domain/interfaces/i-example-service'

const mockTodoService: ITodoService = {
  getTodosByUser: vi.fn(),
  createTodo: vi.fn(),
  updateTodo: vi.fn(),
  toggleComplete: vi.fn(),
  deleteTodo: vi.fn(),
}

const mockUserService: IUserService = {
  getUsers: vi.fn(),
}

const mockExampleService: IExampleService = {
  getGreeting: vi.fn(),
}

vi.mock('../../core/providers', () => ({
  useServices: () => ({
    todoService: mockTodoService,
    userService: mockUserService,
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

describe('useDeleteTodo', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should call todoService.deleteTodo when mutate is invoked', async () => {
    // Arrange
    vi.mocked(mockTodoService.deleteTodo).mockResolvedValue(undefined)
    const { result } = renderHook(() => useDeleteTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate(1))

    // Assert
    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(mockTodoService.deleteTodo).toHaveBeenCalledOnce()
    expect(mockTodoService.deleteTodo).toHaveBeenCalledWith(1)
  })

  it('should set error state when the mutation fails', async () => {
    // Arrange
    vi.mocked(mockTodoService.deleteTodo).mockRejectedValue(new Error('delete failed'))
    const { result } = renderHook(() => useDeleteTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate(1))

    // Assert
    await waitFor(() => expect(result.current.isError).toBe(true))
    expect(result.current.error?.message).toBe('delete failed')
  })
})
