import { ComponentFixture, TestBed } from '@angular/core/testing'
import { TodosPageComponent } from './todos-page.component'
import { TodoStateService } from '../../../state/todos/todo-state.service'
import { AuthStateService } from '../../../state/auth/auth-state.service'
import { signal } from '@angular/core'

describe('TodosPageComponent', () => {
  let component: TodosPageComponent
  let fixture: ComponentFixture<TodosPageComponent>

  beforeEach(async () => {
    const mockTodoState = {
      filter: signal('all' as const),
      filteredTodos: signal([] as any[]),
      todos: { value: signal([]), isLoading: signal(false), error: signal(undefined), reload: jasmine.createSpy('reload'), status: signal('idle'), hasValue: signal(false), destroy: jasmine.createSpy('destroy'), set: jasmine.createSpy('set'), update: jasmine.createSpy('update'), asReadonly: jasmine.createSpy('asReadonly') },
      createTodo: jasmine.createSpy('createTodo').and.resolveTo(),
      toggleTodo: jasmine.createSpy('toggleTodo').and.resolveTo(),
      deleteTodo: jasmine.createSpy('deleteTodo').and.resolveTo(),
      updateTodo: jasmine.createSpy('updateTodo').and.resolveTo(),
    }

    const mockAuthState = {
      currentUser: signal({ userId: 1, username: 'alice', displayName: 'Alice', createdAt: '2024-01-01' }),
      isLoggedIn: signal(true),
    }

    await TestBed.configureTestingModule({
      imports: [TodosPageComponent],
      providers: [
        { provide: TodoStateService, useValue: mockTodoState },
        { provide: AuthStateService, useValue: mockAuthState },
      ],
    }).compileComponents()

    fixture = TestBed.createComponent(TodosPageComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })

  it('should create the component', () => {
    expect(component).toBeTruthy()
  })
})
