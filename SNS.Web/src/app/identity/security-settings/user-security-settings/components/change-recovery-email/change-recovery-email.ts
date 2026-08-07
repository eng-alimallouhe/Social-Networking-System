import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideChevronLeft, LucideCheckLine } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-change-recovery-email',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    LucideChevronLeft,
    LucideCheckLine,
    RouterLink
  ],
  templateUrl: './change-recovery-email.html',
  styleUrl: './change-recovery-email.css'
})
export class ChangeRecoveryEmail {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private securityService = inject(UserSecuritySettingsService);
  private loadingService = inject(LoadingSettingsService);

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loadingService.show();
    const newEmail = this.form.value.email;

    this.securityService.initialRecoveryEmailChange({ newEmail })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            this.router.navigate(
              ['/account-settings/security-settings/verify-recovery-email'],
              {
                state: {
                  userId: res.value.userId,
                  token: res.value.token,
                  newEmail: newEmail
                }
              }
            );
          }
        }
      });
  }
}
