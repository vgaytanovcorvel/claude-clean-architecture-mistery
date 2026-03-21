import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthStateService } from '../../core/auth/auth-state.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, RouterLinkActive],
})
export class NavBarComponent {
  protected readonly authState = inject(AuthStateService);
  private readonly router = inject(Router);

  protected get userInitial(): string {
    return this.authState.session()?.name?.[0]?.toUpperCase() ?? '?';
  }

  protected logout(): void {
    this.authState.clearSession();
    this.router.navigate(['/auth/login']);
  }
}
