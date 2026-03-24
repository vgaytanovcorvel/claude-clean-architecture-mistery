import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor, act } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createElement, type ReactNode } from 'react'
import { useToggleTodo } from './use-toggle-todo'
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

describe('useToggleTodo', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should call todoService.toggleComplete when mutate is invoked', async () => {
    // Arrange
    const toggledTodo: TodoItem = { id: 1, title: 'Buy groceries', isComplete: true, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: '2025-06-16T10:00:00Z' }
    vi.mocked(mockTodoService.toggleComplete).mockResolvedValue(toggledTodo)
    const { result } = renderHook(() => useToggleTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate(1))

    // Assert
    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toEqual(toggledTodo)
    expect(mockTodoService.toggleComplete).toHaveBeenCalledOnce()
    expect(mockTodoService.toggleComplete).toHaveBeenCalledWith(1)
  })

  it('should set error state when the mutation fails', async () => {
    // Arrange
    vi.mocked(mockTodoService.toggleComplete).mockRejectedValue(new Error('toggle failed'))
    const { result } = renderHook(() => useToggleTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate(1))

    // Assert
    await waitFor(() => expect(result.current.isError).toBe(true))
    expect(result.current.error?.message).toBe('toggle failed')
  })
})
