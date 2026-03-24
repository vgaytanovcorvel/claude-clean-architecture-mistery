import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { TodoPage } from './todo-page'
import { ServicesContext, type Services } from '../../core/providers'
import type { User } from '../../domain/models/user'
import type { TodoItem } from '../../domain/models/todo-item'

const fakeUsers: User[] = [
  { id: 1, name: 'Alice', avatarUrl: 'https://example.com/alice.png' },
  { id: 2, name: 'Bob', avatarUrl: 'https://example.com/bob.png' },
]

const fakeTodos: TodoItem[] = [
  { id: 10, title: 'Buy milk', isComplete: false, userId: 1, createdAtUtc: '2026-01-01T00:00:00Z', completedAtUtc: null },
  { id: 11, title: 'Clean house', isComplete: true, userId: 1, createdAtUtc: '2026-01-01T00:00:00Z', completedAtUtc: '2026-01-02T00:00:00Z' },
]

function createMockServices(): Services {
  return {
    exampleService: { getGreeting: vi.fn() } as any,
    userService: {
      getUsers: vi.fn().mockResolvedValue(fakeUsers),
    },
    todoService: {
      getTodosByUser: vi.fn().mockResolvedValue(fakeTodos),
      createTodo: vi.fn().mockResolvedValue(fakeTodos[0]),
      toggleComplete: vi.fn().mockResolvedValue(fakeTodos[0]),
      deleteTodo: vi.fn().mockResolvedValue(undefined),
      updateTodo: vi.fn().mockResolvedValue(fakeTodos[0]),
    },
  } as unknown as Services
}

function renderWithProviders(services: Services) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false, gcTime: 0 } },
  })
  return render(
    <QueryClientProvider client={queryClient}>
      <ServicesContext.Provider value={services}>
        <TodoPage />
      </ServicesContext.Provider>
    </QueryClientProvider>,
  )
}

describe('TodoPage', () => {
  let mockServices: Services

  beforeEach(() => {
    vi.clearAllMocks()
    mockServices = createMockServices()
  })

  it('should render the page heading when mounted', async () => {
    // Arrange + Act
    renderWithProviders(mockServices)

    // Assert
    expect(screen.getByRole('heading', { name: 'Todos' })).toBeInTheDocument()
  })

  it('should render user chips after users load', async () => {
    // Arrange + Act
    renderWithProviders(mockServices)

    // Assert
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /Alice/i })).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /Bob/i })).toBeInTheDocument()
    })
  })

  it('should show hint to select user when no user is selected', async () => {
    // Arrange + Act
    renderWithProviders(mockServices)

    // Assert
    await waitFor(() => {
      expect(screen.getByText(/select a user/i)).toBeInTheDocument()
    })
  })

  it('should load todos when a user is selected', async () => {
    // Arrange
    renderWithProviders(mockServices)
    await waitFor(() => screen.getByRole('button', { name: /Alice/i }))

    // Act
    fireEvent.click(screen.getByRole('button', { name: /Alice/i }))

    // Assert
    await waitFor(() => {
      expect(mockServices.todoService.getTodosByUser).toHaveBeenCalledWith(1)
      expect(screen.getByText('Buy milk')).toBeInTheDocument()
    })
  })
})
