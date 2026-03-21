import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoCardComponent } from './todo-card.component';
import { Todo } from '../../../../domain/todo.model';

describe('TodoCardComponent', () => {
  let component: TodoCardComponent;
  let fixture: ComponentFixture<TodoCardComponent>;

  const fakeTodo: Todo = {
    id: 'todo-1',
    userId: 'user-1',
    title: 'Buy groceries',
    description: 'Milk and eggs',
    priority: 'medium',
    completed: false,
    createdAt: '2024-01-15T10:00:00.000Z',
    updatedAt: '2024-01-15T10:00:00.000Z',
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoCardComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('todo', fakeTodo);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should render the todo title', () => {
    // Assert
    const title = fixture.nativeElement.querySelector('.card__title');
    expect(title?.textContent?.trim()).toBe('Buy groceries');
  });

  it('should render the description when it is provided', () => {
    // Assert
    const desc = fixture.nativeElement.querySelector('.card__description');
    expect(desc?.textContent?.trim()).toBe('Milk and eggs');
  });

  it('should not render the description when it is empty', () => {
    // Arrange
    fixture.componentRef.setInput('todo', { ...fakeTodo, description: '' });
    fixture.detectChanges();

    // Assert
    const desc = fixture.nativeElement.querySelector('.card__description');
    expect(desc).toBeNull();
  });

  it('should apply the completed class when todo is completed', () => {
    // Arrange
    fixture.componentRef.setInput('todo', { ...fakeTodo, completed: true });
    fixture.detectChanges();

    // Assert
    const card = fixture.nativeElement.querySelector('.card');
    expect(card?.classList).toContain('card--completed');
  });

  describe('when the checkbox is clicked', () => {
    it('should emit toggleComplete with the todo', () => {
      // Arrange
      const spy = jasmine.createSpy('toggleComplete');
      component.toggleComplete.subscribe(spy);

      // Act
      const checkbox = fixture.nativeElement.querySelector('.card__checkbox');
      checkbox.click();

      // Assert
      expect(spy).toHaveBeenCalledOnceWith(fakeTodo);
    });
  });

  describe('when the edit button is clicked', () => {
    it('should emit editRequest with the todo', () => {
      // Arrange
      const spy = jasmine.createSpy('editRequest');
      component.editRequest.subscribe(spy);

      // Act
      const editBtn = fixture.nativeElement.querySelector('[aria-label="Edit todo"]');
      editBtn.click();

      // Assert
      expect(spy).toHaveBeenCalledOnceWith(fakeTodo);
    });
  });

  describe('when the delete button is clicked', () => {
    it('should emit deleteRequest with the todo', () => {
      // Arrange
      const spy = jasmine.createSpy('deleteRequest');
      component.deleteRequest.subscribe(spy);

      // Act
      const deleteBtn = fixture.nativeElement.querySelector('[aria-label="Delete todo"]');
      deleteBtn.click();

      // Assert
      expect(spy).toHaveBeenCalledOnceWith(fakeTodo);
    });
  });
});
