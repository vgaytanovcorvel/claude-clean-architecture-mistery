import {
  ChangeDetectionStrategy,
  Component,
  input,
  output,
  inject,
} from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { GlassInputComponent } from '../../../../shared/components/glass-input/glass-input.component';
import { GlassButtonComponent } from '../../../../shared/components/glass-button/glass-button.component';
import { RouterLink } from '@angular/router';

export interface AuthFormSubmit {
  email: string;
  name?: string;
  password: string;
}

@Component({
  selector: 'app-auth-form',
  templateUrl: './auth-form.component.html',
  styleUrl: './auth-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, GlassInputComponent, GlassButtonComponent, RouterLink],
})
export class AuthFormComponent {
  mode = input<'login' | 'signup'>('login');
  isLoading = input<boolean>(false);
  serverError = input<string | null>(null);

  submitted = output<AuthFormSubmit>();

  private readonly fb = inject(FormBuilder);

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    name: [''],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  protected get isSignup(): boolean {
    return this.mode() === 'signup';
  }

  protected getError(field: string): string | null {
    const control = this.form.get(field);
    if (!control?.invalid || !control.touched) return null;
    if (control.hasError('required')) return `${this.capitalize(field)} is required.`;
    if (control.hasError('email')) return 'Please enter a valid email address.';
    if (control.hasError('minlength')) {
      const min = control.getError('minlength').requiredLength;
      return `${this.capitalize(field)} must be at least ${min} characters.`;
    }
    return null;
  }

  protected handleSubmit(): void {
    if (this.isSignup) {
      this.form.get('name')?.setValidators([Validators.required, Validators.minLength(2)]);
      this.form.get('name')?.updateValueAndValidity();
    }
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    const { email, name, password } = this.form.value;
    this.submitted.emit({
      email: email!,
      name: this.isSignup ? name! : undefined,
      password: password!,
    });
  }

  private capitalize(str: string): string {
    return str.charAt(0).toUpperCase() + str.slice(1);
  }
}
