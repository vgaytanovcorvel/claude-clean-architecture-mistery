import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AuthFormComponent } from './auth-form.component';

describe('AuthFormComponent', () => {
  let component: AuthFormComponent;
  let fixture: ComponentFixture<AuthFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuthFormComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AuthFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('in login mode', () => {
    it('should not show the name field', () => {
      // Arrange + Act
      fixture.componentRef.setInput('mode', 'login');
      fixture.detectChanges();

      // Assert
      const nameInput = fixture.nativeElement.querySelector('input[type="text"]');
      expect(nameInput).toBeNull();
    });

    it('should show an email validation error when email is invalid and touched', () => {
      // Arrange
      const emailControl = component.form.get('email')!;
      emailControl.setValue('not-an-email');
      emailControl.markAsTouched();
      fixture.detectChanges();

      // Assert
      const errorEl = fixture.nativeElement.querySelector('[role="alert"]');
      expect(errorEl?.textContent).toContain('valid email');
    });
  });

  describe('in signup mode', () => {
    beforeEach(() => {
      fixture.componentRef.setInput('mode', 'signup');
      fixture.detectChanges();
    });

    it('should show the name field', () => {
      // Assert
      const inputs = fixture.nativeElement.querySelectorAll('input');
      expect(inputs.length).toBeGreaterThanOrEqual(2);
    });
  });

  describe('when form is submitted', () => {
    it('should not emit when the form is invalid', () => {
      // Arrange
      const spy = jasmine.createSpy('submitted');
      component.submitted.subscribe(spy);

      // Act
      const form = fixture.nativeElement.querySelector('form');
      form.dispatchEvent(new Event('submit'));
      fixture.detectChanges();

      // Assert
      expect(spy).not.toHaveBeenCalled();
    });

    it('should emit form values when the form is valid', () => {
      // Arrange
      const spy = jasmine.createSpy('submitted');
      component.submitted.subscribe(spy);

      component.form.setValue({ email: 'alice@example.com', name: '', password: 'pass123' });
      fixture.detectChanges();

      // Act
      const form = fixture.nativeElement.querySelector('form');
      form.dispatchEvent(new Event('submit'));

      // Assert
      expect(spy).toHaveBeenCalledOnceWith(jasmine.objectContaining({
        email: 'alice@example.com',
        password: 'pass123',
      }));
    });
  });
});
