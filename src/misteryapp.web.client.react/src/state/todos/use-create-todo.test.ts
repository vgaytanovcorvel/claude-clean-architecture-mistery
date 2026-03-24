import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor, act } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createElement, type ReactNode } from 'react'
import { useCreateTodo } from './use-create-todo'
import type { TodoItem } from '../../domain/models/todo-item'
import type { CreateTodoRequest } from '../../domain/models/create-todo-request'
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

describe('useCreateTodo', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should call todoService.createTodo when mutate is invoked', async () => {
    // Arrange
    const request: CreateTodoRequest = { title: 'New todo', userId: 1 }
    const createdTodo: TodoItem = { id: 10, title: 'New todo', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null }
    vi.mocked(mockTodoService.createTodo).mockResolvedValue(createdTodo)
    const { result } = renderHook(() => useCreateTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate(request))

    // Assert
    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toEqual(createdTodo)
    expect(mockTodoService.createTodo).toHaveBeenCalledOnce()
    expect(mockTodoService.createTodo).toHaveBeenCalledWith(request)
  })

  it('should set error state when the mutation fails', async () => {
    // Arrange
    vi.mocked(mockTodoService.createTodo).mockRejectedValue(new Error('create failed'))
    const { result } = renderHook(() => useCreateTodo(), { wrapper: createWrapper() })

    // Act
    act(() => result.current.mutate({ title: 'Fail', userId: 1 }))

    // Assert
    await waitFor(() => expect(result.current.isError).toBe(true))
    expect(result.current.error?.message).toBe('create failed')
  })
})
