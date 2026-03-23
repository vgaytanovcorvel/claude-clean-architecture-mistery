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

  describe('onSubmit', () => {
    it('should set error when username is empty', async () => {
      // Arrange
      ;(component as any).username.set('   ')
      ;(component as any).displayName.set('Alice')

      // Act
      await (component as any).onSubmit()

      // Assert
      expect((component as any).error()).toBe('Both fields are required.')
      expect(authStateSpy.login).not.toHaveBeenCalled()
    })

    it('should set error when displayName is empty', async () => {
      // Arrange
      ;(component as any).username.set('alice')
      ;(component as any).displayName.set('   ')

      // Act
      await (component as any).onSubmit()

      // Assert
      expect((component as any).error()).toBe('Both fields are required.')
      expect(authStateSpy.login).not.toHaveBeenCalled()
    })

    it('should call authState.login and navigate to /todos when login succeeds', async () => {
      // Arrange
      ;(component as any).username.set('alice')
      ;(component as any).displayName.set('Alice')
      authStateSpy.login.and.resolveTo()

      // Act
      await (component as any).onSubmit()

      // Assert
      expect(authStateSpy.login).toHaveBeenCalledTimes(1)
      expect(authStateSpy.login).toHaveBeenCalledWith('alice', 'Alice')
      expect(routerSpy.navigate).toHaveBeenCalledTimes(1)
      expect(routerSpy.navigate).toHaveBeenCalledWith(['/todos'])
      expect((component as any).isLoading()).toBeFalse()
    })

    it('should set error message when login fails', async () => {
      // Arrange
      ;(component as any).username.set('alice')
      ;(component as any).displayName.set('Alice')
      authStateSpy.login.and.rejectWith(new Error('Network error'))

      // Act
      await (component as any).onSubmit()

      // Assert
      expect((component as any).error()).toBe('Login failed. Please try again.')
      expect((component as any).isLoading()).toBeFalse()
    })

    it('should set isLoading to true during login and false after', async () => {
      // Arrange
      ;(component as any).username.set('alice')
      ;(component as any).displayName.set('Alice')
      let capturedLoading: boolean | undefined
      authStateSpy.login.and.callFake(async () => {
        capturedLoading = (component as any).isLoading()
      })

      // Act
      await (component as any).onSubmit()

      // Assert
      expect(capturedLoading).toBeTrue()
      expect((component as any).isLoading()).toBeFalse()
    })

    it('should clear previous error when submitting valid credentials', async () => {
      // Arrange — first trigger an error
      ;(component as any).username.set('')
      ;(component as any).displayName.set('')
      await (component as any).onSubmit()
      expect((component as any).error()).toBe('Both fields are required.')

      // Arrange — now set valid credentials
      ;(component as any).username.set('alice')
      ;(component as any).displayName.set('Alice')
      authStateSpy.login.and.resolveTo()

      // Act
      await (component as any).onSubmit()

      // Assert
      expect((component as any).error()).toBeNull()
    })
  })
})
