import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing'
import { TodoInputComponent } from './todo-input.component'
import { FormsModule } from '@angular/forms'

describe('TodoInputComponent', () => {
  let component: TodoInputComponent
  let fixture: ComponentFixture<TodoInputComponent>

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoInputComponent],
    }).compileComponents()

    fixture = TestBed.createComponent(TodoInputComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })

  it('should create the component', () => {
    expect(component).toBeTruthy()
  })

  describe('when the form is submitted with a valid title', () => {
    it('should emit create with the title and null description', () => {
      // Arrange
      const emitted: any[] = []
      component.create.subscribe(val => emitted.push(val))

      const input = fixture.nativeElement.querySelector('input') as HTMLInputElement
      input.value = 'New todo'
      input.dispatchEvent(new Event('input'))
      fixture.detectChanges()

      // Act
      const form = fixture.nativeElement.querySelector('form') as HTMLFormElement
      form.dispatchEvent(new Event('submit'))
      fixture.detectChanges()

      // Assert
      expect(emitted.length).toBe(1)
      expect(emitted[0]).toEqual({ title: 'New todo', description: null })
    })

    it('should clear the input after emitting', fakeAsync(() => {
      // Arrange
      const input = fixture.nativeElement.querySelector('input') as HTMLInputElement
      input.value = 'New todo'
      input.dispatchEvent(new Event('input'))
      fixture.detectChanges()
      tick()

      // Act
      const form = fixture.nativeElement.querySelector('form') as HTMLFormElement
      form.dispatchEvent(new Event('submit'))
      fixture.detectChanges()
      tick()

      // Assert
      expect(input.value).toBe('')
    }))
  })

  describe('when the form is submitted with an empty title', () => {
    it('should not emit create', () => {
      // Arrange
      const emitted: any[] = []
      component.create.subscribe(val => emitted.push(val))

      // Act
      const form = fixture.nativeElement.querySelector('form') as HTMLFormElement
      form.dispatchEvent(new Event('submit'))
      fixture.detectChanges()

      // Assert
      expect(emitted.length).toBe(0)
    })
  })
})
