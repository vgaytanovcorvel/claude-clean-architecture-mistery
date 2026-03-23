import { TestBed } from '@angular/core/testing'
import { TodoStateService } from './todo-state.service'
import { TodoService } from '../../services/todos/todo.service'
import { AuthStateService } from '../auth/auth-state.service'
import { TodoItem } from '../../domain/models/todo-item'
import { signal } from '@angular/core'

describe('TodoStateService', () => {
  let service: TodoStateService
  let todoServiceSpy: jasmine.SpyObj<TodoService>
  let mockAuthState: { currentUser: ReturnType<typeof signal> }

  const fakeTodo: TodoItem = {
    todoItemId: 1,
    userId: 1,
    title: 'Test todo',
    description: null,
    isCompleted: false,
    createdAt: '2024-01-01T00:00:00Z',
    completedAt: null,
  }

  beforeEach(() => {
    todoServiceSpy = jasmine.createSpyObj<TodoService>('TodoService', [
      'getTodos', 'createTodo', 'updateTodo', 'deleteTodo', 'toggleTodo',
    ])

    mockAuthState = {
      currentUser: signal(null),
    }

    TestBed.configureTestingModule({
      providers: [
        TodoStateService,
        { provide: TodoService, useValue: todoServiceSpy },
        { provide: AuthStateService, useValue: mockAuthState },
      ],
    })

    service = TestBed.inject(TodoStateService)
  })

  describe('filter', () => {
    it('should initialize to all when created', () => {
      // Assert
      expect(service.filter()).toBe('all')
    })
  })

  describe('createTodo', () => {
    it('should delegate to TodoService when called', async () => {
      // Arrange
      const request = { title: 'New todo', description: null }
      todoServiceSpy.createTodo.and.resolveTo(fakeTodo)

      // Act
      await service.createTodo(request)

      // Assert
      expect(todoServiceSpy.createTodo).toHaveBeenCalledOnceWith(request)
    })
  })

  describe('updateTodo', () => {
    it('should delegate to TodoService when called', async () => {
      // Arrange
      const request = { title: 'Updated', description: null, isCompleted: false }
      todoServiceSpy.updateTodo.and.resolveTo({ ...fakeTodo, title: 'Updated' })

      // Act
      await service.updateTodo(1, request)

      // Assert
      expect(todoServiceSpy.updateTodo).toHaveBeenCalledOnceWith(1, request)
    })
  })

  describe('deleteTodo', () => {
    it('should delegate to TodoService when called', async () => {
      // Arrange
      todoServiceSpy.deleteTodo.and.resolveTo()

      // Act
      await service.deleteTodo(1)

      // Assert
      expect(todoServiceSpy.deleteTodo).toHaveBeenCalledOnceWith(1)
    })
  })

  describe('toggleTodo', () => {
    it('should delegate to TodoService when called', async () => {
      // Arrange
      todoServiceSpy.toggleTodo.and.resolveTo({ ...fakeTodo, isCompleted: true })

      // Act
      await service.toggleTodo(1)

      // Assert
      expect(todoServiceSpy.toggleTodo).toHaveBeenCalledOnceWith(1)
    })
  })
})
