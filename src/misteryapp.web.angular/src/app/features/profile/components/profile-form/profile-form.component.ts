import { ChangeDetectionStrategy, Component, input, output, inject, OnChanges } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { User, UpdateProfileRequest } from '../../../../domain/user.model';
import { GlassInputComponent } from '../../../../shared/components/glass-input/glass-input.component';
import { GlassButtonComponent } from '../../../../shared/components/glass-button/glass-button.component';

@Component({
  selector: 'app-profile-form',
  templateUrl: './profile-form.component.html',
  styleUrl: './profile-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, GlassInputComponent, GlassButtonComponent],
})
export class ProfileFormComponent implements OnChanges {
  user = input.required<User>();
  isLoading = input<boolean>(false);
  serverError = input<string | null>(null);

  submitted = output<UpdateProfileRequest>();
  cancelled = output<void>();

  private readonly fb = inject(FormBuilder);

  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
  });

  ngOnChanges(): void {
    this.form.patchValue({ name: this.user().name });
  }

  protected getError(field: string): string | null {
    const control = this.form.get(field);
    if (!control?.invalid || !control.touched) return null;
    if (control.hasError('required')) return 'Name is required.';
    if (control.hasError('minlength')) return 'Name must be at least 2 characters.';
    if (control.hasError('maxlength')) return 'Name must be 50 characters or fewer.';
    return null;
  }

  protected handleSubmit(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    this.submitted.emit({ name: this.form.value.name! });
  }
}
