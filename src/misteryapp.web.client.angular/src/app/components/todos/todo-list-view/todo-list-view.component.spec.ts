import { ComponentFixture, TestBed } from '@angular/core/testing'
import { TodoListViewComponent } from './todo-list-view.component'
import { TodoItem } from '../../../domain/models/todo-item'

describe('TodoListViewComponent', () => {
  let component: TodoListViewComponent
  let fixture: ComponentFixture<TodoListViewComponent>

  const fakeTodos: TodoItem[] = [
    {
      todoItemId: 1,
      userId: 1,
      title: 'First todo',
      description: null,
      isCompleted: false,
      createdAt: '2024-01-01T00:00:00Z',
      completedAt: null,
    },
    {
      todoItemId: 2,
      userId: 1,
      title: 'Second todo',
      description: null,
      isCompleted: true,
      createdAt: '2024-01-02T00:00:00Z',
      completedAt: '2024-01-03T00:00:00Z',
    },
  ]

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoListViewComponent],
    }).compileComponents()

    fixture = TestBed.createComponent(TodoListViewComponent)
    component = fixture.componentInstance
    fixture.componentRef.setInput('todos', fakeTodos)
    fixture.detectChanges()
  })

  it('should render a todo item for each todo', () => {
    // Assert
    const items = fixture.nativeElement.querySelectorAll('app-todo-item')
    expect(items.length).toBe(2)
  })

  it('should render the empty state when todos is an empty array', () => {
    // Arrange
    fixture.componentRef.setInput('todos', [])
    fixture.detectChanges()

    // Assert
    const empty = fixture.nativeElement.querySelector('.todo-list__empty')
    expect(empty).toBeTruthy()
    expect(empty.textContent).toContain('No todos yet')
  })

  it('should render the loading state when isLoading is true', () => {
    // Arrange
    fixture.componentRef.setInput('isLoading', true)
    fixture.detectChanges()

    // Assert
    const loading = fixture.nativeElement.querySelector('.todo-list__loading')
    expect(loading).toBeTruthy()
    expect(loading.textContent).toContain('Loading')
  })

  it('should not render the loading state when isLoading is false', () => {
    // Assert
    const loading = fixture.nativeElement.querySelector('.todo-list__loading')
    expect(loading).toBeNull()
  })

  describe('when a todo item emits toggle', () => {
    it('should re-emit toggle with the todo id', () => {
      // Arrange
      const emitted: number[] = []
      component.toggle.subscribe(val => emitted.push(val))

      // Act
      const checkbox = fixture.nativeElement.querySelector('.todo__checkbox') as HTMLButtonElement
      checkbox.click()

      // Assert
      expect(emitted).toEqual([1])
    })
  })

  describe('when a todo item emits delete', () => {
    it('should re-emit delete with the todo id', () => {
      // Arrange
      const emitted: number[] = []
      component.delete.subscribe(val => emitted.push(val))

      // Act
      const deleteBtn = fixture.nativeElement.querySelector('.todo__delete') as HTMLButtonElement
      deleteBtn.click()

      // Assert
      expect(emitted).toEqual([1])
    })
  })
})
