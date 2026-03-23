import { TestBed } from '@angular/core/testing'
import { Router, UrlTree } from '@angular/router'
import { authGuard } from './auth.guard'
import { AuthStateService } from '../../state/auth/auth-state.service'
import { signal } from '@angular/core'

describe('authGuard', () => {
  let routerSpy: jasmine.SpyObj<Router>

  function runGuard(isLoggedIn: boolean): boolean | UrlTree {
    const mockAuthState = { isLoggedIn: signal(isLoggedIn) }
    routerSpy = jasmine.createSpyObj<Router>('Router', ['createUrlTree'])
    routerSpy.createUrlTree.and.returnValue({ toString: () => '/login' } as UrlTree)

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthStateService, useValue: mockAuthState },
        { provide: Router, useValue: routerSpy },
      ],
    })

    return TestBed.runInInjectionContext(() => authGuard({} as any, {} as any)) as boolean | UrlTree
  }

  it('should return true when the user is logged in', () => {
    // Act
    const result = runGuard(true)

    // Assert
    expect(result).toBeTrue()
  })

  it('should redirect to /login when the user is not logged in', () => {
    // Act
    runGuard(false)

    // Assert
    expect(routerSpy.createUrlTree).toHaveBeenCalledTimes(1)
    expect(routerSpy.createUrlTree).toHaveBeenCalledWith(['/login'])
  })
})
