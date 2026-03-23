import { TestBed } from '@angular/core/testing'
import { provideHttpClient } from '@angular/common/http'
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing'
import { UserApiService } from './user-api.service'
import { API_BASE_URL } from '../../app.config'
import { User } from '../../domain/models/user'
import { ApiResponse } from '../../domain/models/api-response'

describe('UserApiService', () => {
  let service: UserApiService
  let httpTesting: HttpTestingController

  const fakeUser: User = {
    userId: 1,
    username: 'alice',
    displayName: 'Alice',
    createdAt: '2024-01-01T00:00:00Z',
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        UserApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: '/api' },
      ],
    })

    service = TestBed.inject(UserApiService)
    httpTesting = TestBed.inject(HttpTestingController)
  })

  afterEach(() => {
    httpTesting.verify()
  })

  describe('login', () => {
    it('should return the user when the API responds with success', async () => {
      // Arrange
      const response: ApiResponse<User> = { success: true, data: fakeUser, error: null, statusCode: 200 }
      const promise = service.login({ username: 'alice', displayName: 'Alice' })

      // Act
      const req = httpTesting.expectOne('/api/users/login')
      expect(req.request.method).toBe('POST')
      expect(req.request.body).toEqual({ username: 'alice', displayName: 'Alice' })
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toEqual(fakeUser)
    })
  })

  describe('getUserByUsername', () => {
    it('should return the user when the user exists', async () => {
      // Arrange
      const response: ApiResponse<User> = { success: true, data: fakeUser, error: null, statusCode: 200 }
      const promise = service.getUserByUsername('alice')

      // Act
      const req = httpTesting.expectOne('/api/users/alice')
      expect(req.request.method).toBe('GET')
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toEqual(fakeUser)
    })

    it('should return null when the user does not exist', async () => {
      // Arrange
      const response: ApiResponse<User> = { success: false, data: null, error: 'Not found', statusCode: 404 }
      const promise = service.getUserByUsername('nobody')

      // Act
      const req = httpTesting.expectOne('/api/users/nobody')
      req.flush(response)

      // Assert
      const result = await promise
      expect(result).toBeNull()
    })
  })
})
