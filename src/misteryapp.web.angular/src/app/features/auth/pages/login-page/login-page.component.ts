import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { AuthFormComponent, AuthFormSubmit } from '../../components/auth-form/auth-form.component';
import { GlassCardComponent } from '../../../../shared/components/glass-card/glass-card.component';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [AuthFormComponent, GlassCardComponent],
})
export class LoginPageComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly isLoading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected handleSubmit(data: AuthFormSubmit): void {
    this.isLoading.set(true);
    this.error.set(null);

    const result = this.authService.login({ email: data.email, password: data.password });

    this.isLoading.set(false);

    if (!result.success) {
      this.error.set(result.error);
      return;
    }

    this.router.navigate(['/todos']);
  }
}
