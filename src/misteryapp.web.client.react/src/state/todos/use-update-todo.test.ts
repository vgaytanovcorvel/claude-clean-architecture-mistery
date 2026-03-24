import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor, act } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createElement, type ReactNode } from 'react'
import { useUpdateTodo } from './use-update-todo'
import type { TodoItem } from '../../domain/models/todo-item'
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

describe('useUpdateTodo', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should call todoService.updateTodo when mutate is invoked', async () => {
    // Arrange
    const updatedTodo: TodoItem = { id: 1, title: 'Updated', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null }
    vi.mocked(mockTodoService.updateTodo).mockResolvedValue(updatedTodo)
    const { result } = renderHook(() => useUpdateTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate({ id: 1, data: { title: 'Updated' } }))

    // Assert
    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toEqual(updatedTodo)
    expect(mockTodoService.updateTodo).toHaveBeenCalledOnce()
    expect(mockTodoService.updateTodo).toHaveBeenCalledWith(1, { title: 'Updated' })
  })

  it('should set error state when the mutation fails', async () => {
    // Arrange
    vi.mocked(mockTodoService.updateTodo).mockRejectedValue(new Error('update failed'))
    const { result } = renderHook(() => useUpdateTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate({ id: 1, data: { title: 'Fail' } }))

    // Assert
    await waitFor(() => expect(result.current.isError).toBe(true))
    expect(result.current.error?.message).toBe('update failed')
  })
})
