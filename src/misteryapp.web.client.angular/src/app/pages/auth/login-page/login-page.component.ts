import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core'
import { FormsModule } from '@angular/forms'
import { Router } from '@angular/router'
import { AuthStateService } from '../../../state/auth/auth-state.service'

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule],
})
export class LoginPageComponent {
  private readonly authState = inject(AuthStateService)
  private readonly router = inject(Router)

  protected username = signal('')
  protected displayName = signal('')
  protected isLoading = signal(false)
  protected error = signal<string | null>(null)

  protected async onSubmit() {
    const u = this.username().trim()
    const d = this.displayName().trim()
    if (!u || !d) {
      this.error.set('Both fields are required.')
      return
    }

    this.isLoading.set(true)
    this.error.set(null)

    try {
      await this.authState.login(u, d)
      this.router.navigate(['/todos'])
    } catch {
      this.error.set('Login failed. Please try again.')
    } finally {
      this.isLoading.set(false)
    }
  }
}
