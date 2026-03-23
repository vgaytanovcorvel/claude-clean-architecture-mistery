import { TestBed } from '@angular/core/testing'
import { provideHttpClient } from '@angular/common/http'
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing'
import { TodoApiService } from './todo-api.service'
import { API_BASE_URL } from '../../app.config'
import { TodoItem } from '../../domain/models/todo-item'
import { ApiResponse } from '../../domain/models/api-response'

describe('TodoApiService', () => {
  let service: TodoApiService
  let httpTesting: HttpTestingController

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
    TestBed.configureTestingModule({
      providers: [
        TodoApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: '/api' },
      ],
    })

    service = TestBed.inject(TodoApiService)
    httpTesting = TestBed.inject(HttpTestingController)
  })

  afterEach(() => {
    httpTesting.verify()
  })

  describe('todoGetAll', () => {
    it('should return todos when the API responds with data', async () => {
      // Arrange
      const response: ApiResponse<TodoItem[]> = { success: true, data: [fakeTodo], error: null, statusCode: 200 }
      const promise = service.todoGetAll('alice')

      // Act
      const req = httpTesting.expectOne('/api/todos?username=alice')
      expect(req.request.method).toBe('GET')
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toEqual([fakeTodo])
    })

    it('should return an empty array when data is null', async () => {
      // Arrange
      const response: ApiResponse<TodoItem[]> = { success: true, data: null, error: null, statusCode: 200 }
      const promise = service.todoGetAll('alice')

      // Act
      const req = httpTesting.expectOne('/api/todos?username=alice')
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toEqual([])
    })
  })

  describe('todoCreate', () => {
    it('should return the created todo when the API responds with success', async () => {
      // Arrange
      const request = { title: 'New todo', description: null }
      const response: ApiResponse<TodoItem> = { success: true, data: fakeTodo, error: null, statusCode: 201 }
      const promise = service.todoCreate('alice', request)

      // Act
      const req = httpTesting.expectOne('/api/todos?username=alice')
      expect(req.request.method).toBe('POST')
      expect(req.request.body).toEqual(request)
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toEqual(fakeTodo)
    })
  })

  describe('todoUpdate', () => {
    it('should return the updated todo when the API responds with success', async () => {
      // Arrange
      const request = { title: 'Updated', description: null, isCompleted: false }
      const updatedTodo = { ...fakeTodo, title: 'Updated' }
      const response: ApiResponse<TodoItem> = { success: true, data: updatedTodo, error: null, statusCode: 200 }
      const promise = service.todoUpdate(1, 'alice', request)

      // Act
      const req = httpTesting.expectOne('/api/todos/1?username=alice')
      expect(req.request.method).toBe('PUT')
      expect(req.request.body).toEqual(request)
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toEqual(updatedTodo)
    })
  })

  describe('todoDelete', () => {
    it('should complete without error when the API responds with success', async () => {
      // Arrange
      const response: ApiResponse<void> = { success: true, data: null, error: null, statusCode: 204 }
      const promise = service.todoDelete(1, 'alice')

      // Act
      const req = httpTesting.expectOne('/api/todos/1?username=alice')
      expect(req.request.method).toBe('DELETE')
      req.flush(response)

      // Assert
      await promise
    })
  })

  describe('todoToggle', () => {
    it('should return the toggled todo when the API responds with success', async () => {
      // Arrange
      const toggled = { ...fakeTodo, isCompleted: true }
      const response: ApiResponse<TodoItem> = { success: true, data: toggled, error: null, statusCode: 200 }
      const promise = service.todoToggle(1, 'alice')

      // Act
      const req = httpTesting.expectOne('/api/todos/1/toggle?username=alice')
      expect(req.request.method).toBe('PATCH')
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toEqual(toggled)
    })
  })
})
