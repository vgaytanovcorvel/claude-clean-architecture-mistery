import { ChangeDetectionStrategy, Component, inject } from '@angular/core'
import { AuthStateService } from '../../../state/auth/auth-state.service'

@Component({
  selector: 'app-layout',
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppLayoutComponent {
  protected readonly authState = inject(AuthStateService)
}
