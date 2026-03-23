import { ComponentFixture, TestBed } from '@angular/core/testing'
import { LoginPageComponent } from './login-page.component'
import { AuthStateService } from '../../../state/auth/auth-state.service'
import { Router } from '@angular/router'
import { FormsModule } from '@angular/forms'

describe('LoginPageComponent', () => {
  let component: LoginPageComponent
  let fixture: ComponentFixture<LoginPageComponent>
  let authStateSpy: jasmine.SpyObj<AuthStateService>
  let routerSpy: jasmine.SpyObj<Router>

  beforeEach(async () => {
    authStateSpy = jasmine.createSpyObj<AuthStateService>('AuthStateService', ['login'])
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigate'])

    await TestBed.configureTestingModule({
      imports: [LoginPageComponent],
      providers: [
        { provide: AuthStateService, useValue: authStateSpy },
        { provide: Router, useValue: routerSpy },
      ],
    }).compileComponents()

    fixture = TestBed.createComponent(LoginPageComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })

  it('should create the component', () => {
    expect(component).toBeTruthy()
  })
})
