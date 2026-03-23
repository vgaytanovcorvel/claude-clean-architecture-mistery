import { ComponentFixture, TestBed } from '@angular/core/testing'
import { TodoItemComponent } from './todo-item.component'
import { TodoItem } from '../../../domain/models/todo-item'

describe('TodoItemComponent', () => {
  let component: TodoItemComponent
  let fixture: ComponentFixture<TodoItemComponent>

  const fakeTodo: TodoItem = {
    todoItemId: 42,
    userId: 1,
    title: 'Test todo',
    description: 'A description',
    isCompleted: false,
    createdAt: '2024-01-01T00:00:00Z',
    completedAt: null,
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoItemComponent],
    }).compileComponents()

    fixture = TestBed.createComponent(TodoItemComponent)
    component = fixture.componentInstance
    fixture.componentRef.setInput('todo', fakeTodo)
    fixture.detectChanges()
  })

  it('should render the todo title', () => {
    // Assert
    const title = fixture.nativeElement.querySelector('.todo__title')
    expect(title.textContent).toContain('Test todo')
  })

  it('should render the description when it exists', () => {
    // Assert
    const desc = fixture.nativeElement.querySelector('.todo__description')
    expect(desc.textContent).toContain('A description')
  })

  it('should not render the description when it is null', () => {
    // Arrange
    fixture.componentRef.setInput('todo', { ...fakeTodo, description: null })
    fixture.detectChanges()

    // Assert
    const desc = fixture.nativeElement.querySelector('.todo__description')
    expect(desc).toBeNull()
  })

  it('should add the completed class when todo is completed', () => {
    // Arrange
    fixture.componentRef.setInput('todo', { ...fakeTodo, isCompleted: true })
    fixture.detectChanges()

    // Assert
    const el = fixture.nativeElement.querySelector('.todo')
    expect(el.classList.contains('todo--completed')).toBeTrue()
  })

  describe('when the checkbox is clicked', () => {
    it('should emit toggle with the todo id', () => {
      // Arrange
      const emitted: number[] = []
      component.toggle.subscribe(val => emitted.push(val))

      // Act
      const checkbox = fixture.nativeElement.querySelector('.todo__checkbox') as HTMLButtonElement
      checkbox.click()

      // Assert
      expect(emitted).toEqual([42])
    })
  })

  describe('when the delete button is clicked', () => {
    it('should emit delete with the todo id', () => {
      // Arrange
      const emitted: number[] = []
      component.delete.subscribe(val => emitted.push(val))

      // Act
      const deleteBtn = fixture.nativeElement.querySelector('.todo__delete') as HTMLButtonElement
      deleteBtn.click()

      // Assert
      expect(emitted).toEqual([42])
    })
  })
})
