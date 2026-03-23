import { TestBed } from '@angular/core/testing'
import { TodoService } from './todo.service'
import { TodoApiService } from '../../repositories/todos/todo-api.service'
import { AuthService } from '../auth/auth.service'
import { TodoItem } from '../../domain/models/todo-item'
import { User } from '../../domain/models/user'

describe('TodoService', () => {
  let service: TodoService
  let todoApiSpy: jasmine.SpyObj<TodoApiService>
  let authServiceSpy: jasmine.SpyObj<AuthService>

  const fakeUser: User = {
    userId: 1,
    username: 'alice',
    displayName: 'Alice',
    createdAt: '2024-01-01T00:00:00Z',
  }

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
    todoApiSpy = jasmine.createSpyObj<TodoApiService>('TodoApiService', [
      'todoGetAll', 'todoCreate', 'todoUpdate', 'todoDelete', 'todoToggle',
    ])
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['getCurrentUser'])
    authServiceSpy.getCurrentUser.and.returnValue(fakeUser)

    TestBed.configureTestingModule({
      providers: [
        TodoService,
        { provide: TodoApiService, useValue: todoApiSpy },
        { provide: AuthService, useValue: authServiceSpy },
      ],
    })

    service = TestBed.inject(TodoService)
  })

  describe('getTodos', () => {
    it('should return todos for the current user when called', async () => {
      // Arrange
      todoApiSpy.todoGetAll.and.resolveTo([fakeTodo])

      // Act
      const result = await service.getTodos()

      // Assert
      expect(result).toEqual([fakeTodo])
      expect(todoApiSpy.todoGetAll).toHaveBeenCalledOnceWith('alice')
    })
  })

  describe('createTodo', () => {
    it('should create a todo for the current user when called', async () => {
      // Arrange
      const request = { title: 'New todo', description: null }
      todoApiSpy.todoCreate.and.resolveTo(fakeTodo)

      // Act
      const result = await service.createTodo(request)

      // Assert
      expect(result).toEqual(fakeTodo)
      expect(todoApiSpy.todoCreate).toHaveBeenCalledOnceWith('alice', request)
    })
  })

  describe('updateTodo', () => {
    it('should update the todo for the current user when called', async () => {
      // Arrange
      const request = { title: 'Updated', description: null, isCompleted: false }
      todoApiSpy.todoUpdate.and.resolveTo({ ...fakeTodo, title: 'Updated' })

      // Act
      const result = await service.updateTodo(1, request)

      // Assert
      expect(result).toEqual({ ...fakeTodo, title: 'Updated' })
      expect(todoApiSpy.todoUpdate).toHaveBeenCalledOnceWith(1, 'alice', request)
    })
  })

  describe('deleteTodo', () => {
    it('should delete the todo for the current user when called', async () => {
      // Arrange
      todoApiSpy.todoDelete.and.resolveTo()

      // Act
      await service.deleteTodo(1)

      // Assert
      expect(todoApiSpy.todoDelete).toHaveBeenCalledOnceWith(1, 'alice')
    })
  })

  describe('toggleTodo', () => {
    it('should toggle the todo for the current user when called', async () => {
      // Arrange
      const toggled = { ...fakeTodo, isCompleted: true }
      todoApiSpy.todoToggle.and.resolveTo(toggled)

      // Act
      const result = await service.toggleTodo(1)

      // Assert
      expect(result).toEqual(toggled)
      expect(todoApiSpy.todoToggle).toHaveBeenCalledOnceWith(1, 'alice')
    })
  })
})
