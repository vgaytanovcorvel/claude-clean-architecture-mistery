import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createElement, type ReactNode } from 'react'
import { useTodoList } from './use-todo-list'
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

describe('useTodoList', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should return todos when userId is provided and query succeeds', async () => {
    // Arrange
    const expectedTodos: readonly TodoItem[] = [
      { id: 1, title: 'Buy groceries', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null },
    ]
    vi.mocked(mockTodoService.getTodosByUser).mockResolvedValue(expectedTodos)

    // Act
    const { result } = renderHook(() => useTodoList(1), { wrapper: createWrapper() })

    // Assert
    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toEqual(expectedTodos)
    expect(mockTodoService.getTodosByUser).toHaveBeenCalledOnce()
    expect(mockTodoService.getTodosByUser).toHaveBeenCalledWith(1)
  })

  it('should not fetch when userId is null', () => {
    // Arrange + Act
    const { result } = renderHook(() => useTodoList(null), { wrapper: createWrapper() })

    // Assert
    expect(result.current.fetchStatus).toBe('idle')
    expect(mockTodoService.getTodosByUser).not.toHaveBeenCalled()
  })

  it('should return error when the query fails', async () => {
    // Arrange
    vi.mocked(mockTodoService.getTodosByUser).mockRejectedValue(new Error('fetch failed'))

    // Act
    const { result } = renderHook(() => useTodoList(1), { wrapper: createWrapper() })

    // Assert
    await waitFor(() => expect(result.current.isError).toBe(true))
    expect(result.current.error?.message).toBe('fetch failed')
  })
})
