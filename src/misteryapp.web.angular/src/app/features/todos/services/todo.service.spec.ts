import { TestBed } from '@angular/core/testing';
import { TodoService } from './todo.service';
import { TodosMockApiService } from '../api/todos-mock-api.service';
import { Todo } from '../../../domain/todo.model';

describe('TodoService', () => {
  let service: TodoService;
  let mockApi: jasmine.SpyObj<TodosMockApiService>;

  const fakeTodo: Todo = {
    id: 'todo-1',
    userId: 'user-1',
    title: 'Buy groceries',
    description: '',
    priority: 'medium',
    completed: false,
    createdAt: '2024-01-01T00:00:00.000Z',
    updatedAt: '2024-01-01T00:00:00.000Z',
  };

  beforeEach(() => {
    mockApi = jasmine.createSpyObj('TodosMockApiService', [
      'todoFindAllByUserId',
      'todoCreate',
      'todoUpdate',
      'todoDelete',
    ]);

    TestBed.configureTestingModule({
      providers: [
        TodoService,
        { provide: TodosMockApiService, useValue: mockApi },
      ],
    });

    service = TestBed.inject(TodoService);
  });

  describe('getTodosForUser', () => {
    it('should return todos for the given user', () => {
      // Arrange
      mockApi.todoFindAllByUserId.and.returnValue([fakeTodo]);

      // Act
      const result = service.getTodosForUser('user-1');

      // Assert
      expect(result).toEqual([fakeTodo]);
      expect(mockApi.todoFindAllByUserId).toHaveBeenCalledOnceWith('user-1');
    });
  });

  describe('createTodo', () => {
    it('should return failure when title is empty', () => {
      // Act
      const result = service.createTodo('user-1', { title: '', description: '', priority: 'low' });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('required');
    });

    it('should return the created todo when request is valid', () => {
      // Arrange
      mockApi.todoCreate.and.returnValue(fakeTodo);

      // Act
      const result = service.createTodo('user-1', {
        title: 'Buy groceries',
        description: '',
        priority: 'medium',
      });

      // Assert
      expect(result.success).toBeTrue();
      if (result.success) expect(result.value).toEqual(fakeTodo);
      expect(mockApi.todoCreate).toHaveBeenCalledTimes(1);
    });
  });

  describe('updateTodo', () => {
    it('should return failure when todo is not found', () => {
      // Arrange
      mockApi.todoUpdate.and.returnValue(null);

      // Act
      const result = service.updateTodo('todo-1', { title: 'Updated' });

      // Assert
      expect(result.success).toBeFalse();
      if (!result.success) expect(result.error).toContain('not found');
    });

    it('should return the updated todo when todo exists', () => {
      // Arrange
      const updated = { ...fakeTodo, title: 'Updated title' };
      mockApi.todoUpdate.and.returnValue(updated);

      // Act
      const result = service.updateTodo('todo-1', { title: 'Updated title' });

      // Assert
      expect(result.success).toBeTrue();
      if (result.success) expect(result.value.title).toBe('Updated title');
    });
  });

  describe('deleteTodo', () => {
    it('should call the api delete method', () => {
      // Act
      service.deleteTodo('todo-1');

      // Assert
      expect(mockApi.todoDelete).toHaveBeenCalledOnceWith('todo-1');
    });
  });

  describe('toggleComplete', () => {
    it('should toggle completed from false to true', () => {
      // Arrange
      const toggled = { ...fakeTodo, completed: true };
      mockApi.todoUpdate.and.returnValue(toggled);

      // Act
      const result = service.toggleComplete('todo-1', false);

      // Assert
      expect(result.success).toBeTrue();
      if (result.success) expect(result.value.completed).toBeTrue();
      expect(mockApi.todoUpdate).toHaveBeenCalledOnceWith('todo-1', { completed: true });
    });
  });
});
