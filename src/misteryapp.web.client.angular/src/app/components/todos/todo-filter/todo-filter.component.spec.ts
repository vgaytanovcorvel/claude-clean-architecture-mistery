import { ComponentFixture, TestBed } from '@angular/core/testing'
import { TodoFilterComponent } from './todo-filter.component'

describe('TodoFilterComponent', () => {
  let component: TodoFilterComponent
  let fixture: ComponentFixture<TodoFilterComponent>

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoFilterComponent],
    }).compileComponents()

    fixture = TestBed.createComponent(TodoFilterComponent)
    component = fixture.componentInstance
    fixture.componentRef.setInput('filter', 'all')
    fixture.detectChanges()
  })

  it('should render three filter buttons', () => {
    // Assert
    const buttons = fixture.nativeElement.querySelectorAll('.filter__button')
    expect(buttons.length).toBe(3)
  })

  it('should highlight the active filter button', () => {
    // Assert
    const buttons = fixture.nativeElement.querySelectorAll('.filter__button')
    expect(buttons[0].classList.contains('filter__button--active')).toBeTrue()
    expect(buttons[1].classList.contains('filter__button--active')).toBeFalse()
    expect(buttons[2].classList.contains('filter__button--active')).toBeFalse()
  })

  it('should highlight the completed filter when filter is completed', () => {
    // Arrange
    fixture.componentRef.setInput('filter', 'completed')
    fixture.detectChanges()

    // Assert
    const buttons = fixture.nativeElement.querySelectorAll('.filter__button')
    expect(buttons[0].classList.contains('filter__button--active')).toBeFalse()
    expect(buttons[2].classList.contains('filter__button--active')).toBeTrue()
  })

  describe('when a filter button is clicked', () => {
    it('should emit filterChange with all when All is clicked', () => {
      // Arrange
      const emitted: string[] = []
      component.filterChange.subscribe(val => emitted.push(val))

      // Act
      const buttons = fixture.nativeElement.querySelectorAll('.filter__button')
      buttons[0].click()

      // Assert
      expect(emitted).toEqual(['all'])
    })

    it('should emit filterChange with active when Active is clicked', () => {
      // Arrange
      const emitted: string[] = []
      component.filterChange.subscribe(val => emitted.push(val))

      // Act
      const buttons = fixture.nativeElement.querySelectorAll('.filter__button')
      buttons[1].click()

      // Assert
      expect(emitted).toEqual(['active'])
    })

    it('should emit filterChange with completed when Completed is clicked', () => {
      // Arrange
      const emitted: string[] = []
      component.filterChange.subscribe(val => emitted.push(val))

      // Act
      const buttons = fixture.nativeElement.querySelectorAll('.filter__button')
      buttons[2].click()

      // Assert
      expect(emitted).toEqual(['completed'])
    })
  })
})
