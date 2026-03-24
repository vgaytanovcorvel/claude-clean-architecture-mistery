import { describe, it, expect, vi, beforeEach } from 'vitest'
import { TodoService } from './todo-service'
import type { ITodoRepository } from '../domain/interfaces/i-todo-repository'
import type { TodoItem } from '../domain/models/todo-item'

const mockTodoRepository: ITodoRepository = {
  todoFindByUserId: vi.fn(),
  todoCreate: vi.fn(),
  todoUpdate: vi.fn(),
  todoToggle: vi.fn(),
  todoDelete: vi.fn(),
}

describe('TodoService', () => {
  let todoService: TodoService

  beforeEach(() => {
    vi.clearAllMocks()
    todoService = new TodoService(mockTodoRepository)
  })

  describe('getTodosByUser', () => {
    it('should return todos when the user has todos', async () => {
      // Arrange
      const userId = 1
      const expectedTodos: readonly TodoItem[] = [
        { id: 1, title: 'Buy groceries', isComplete: false, userId, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null },
        { id: 2, title: 'Read a book', isComplete: true, userId, createdAtUtc: '2025-06-14T10:00:00Z', completedAtUtc: '2025-06-15T08:00:00Z' },
      ]
      vi.mocked(mockTodoRepository.todoFindByUserId).mockResolvedValue(expectedTodos)

      // Act
      const result = await todoService.getTodosByUser(userId)

      // Assert
      expect(result).toEqual(expectedTodos)
      expect(mockTodoRepository.todoFindByUserId).toHaveBeenCalledOnce()
      expect(mockTodoRepository.todoFindByUserId).toHaveBeenCalledWith(userId)
    })
  })

  describe('createTodo', () => {
    it('should return the created todo when the request is valid', async () => {
      // Arrange
      const request = { title: 'New todo', userId: 1 }
      const createdTodo: TodoItem = { id: 10, title: 'New todo', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null }
      vi.mocked(mockTodoRepository.todoCreate).mockResolvedValue(createdTodo)

      // Act
      const result = await todoService.createTodo(request)

      // Assert
      expect(result).toEqual(createdTodo)
      expect(mockTodoRepository.todoCreate).toHaveBeenCalledOnce()
      expect(mockTodoRepository.todoCreate).toHaveBeenCalledWith(request)
    })
  })

  describe('updateTodo', () => {
    it('should return the updated todo when the todo exists', async () => {
      // Arrange
      const id = 1
      const request = { title: 'Updated title' }
      const updatedTodo: TodoItem = { id, title: 'Updated title', isComplete: false, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: null }
      vi.mocked(mockTodoRepository.todoUpdate).mockResolvedValue(updatedTodo)

      // Act
      const result = await todoService.updateTodo(id, request)

      // Assert
      expect(result).toEqual(updatedTodo)
      expect(mockTodoRepository.todoUpdate).toHaveBeenCalledOnce()
      expect(mockTodoRepository.todoUpdate).toHaveBeenCalledWith(id, request)
    })
  })

  describe('toggleComplete', () => {
    it('should return the toggled todo when the todo exists', async () => {
      // Arrange
      const id = 1
      const toggledTodo: TodoItem = { id, title: 'Buy groceries', isComplete: true, userId: 1, createdAtUtc: '2025-06-15T10:00:00Z', completedAtUtc: '2025-06-16T10:00:00Z' }
      vi.mocked(mockTodoRepository.todoToggle).mockResolvedValue(toggledTodo)

      // Act
      const result = await todoService.toggleComplete(id)

      // Assert
      expect(result).toEqual(toggledTodo)
      expect(mockTodoRepository.todoToggle).toHaveBeenCalledOnce()
      expect(mockTodoRepository.todoToggle).toHaveBeenCalledWith(id)
    })
  })

  describe('deleteTodo', () => {
    it('should call todoDelete when the todo exists', async () => {
      // Arrange
      const id = 1
      vi.mocked(mockTodoRepository.todoDelete).mockResolvedValue(undefined)

      // Act
      await todoService.deleteTodo(id)

      // Assert
      expect(mockTodoRepository.todoDelete).toHaveBeenCalledOnce()
      expect(mockTodoRepository.todoDelete).toHaveBeenCalledWith(id)
    })
  })
})
