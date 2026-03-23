import { ComponentFixture, TestBed } from '@angular/core/testing'
import { AppLayoutComponent } from './app-layout.component'
import { AuthStateService } from '../../../state/auth/auth-state.service'
import { signal } from '@angular/core'

describe('AppLayoutComponent', () => {
  let component: AppLayoutComponent
  let fixture: ComponentFixture<AppLayoutComponent>

  beforeEach(async () => {
    const mockAuthState = {
      currentUser: signal(null),
      isLoggedIn: signal(false),
      logout: jasmine.createSpy('logout'),
    }

    await TestBed.configureTestingModule({
      imports: [AppLayoutComponent],
      providers: [
        { provide: AuthStateService, useValue: mockAuthState },
      ],
    }).compileComponents()

    fixture = TestBed.createComponent(AppLayoutComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })

  it('should create the component', () => {
    expect(component).toBeTruthy()
  })
})
