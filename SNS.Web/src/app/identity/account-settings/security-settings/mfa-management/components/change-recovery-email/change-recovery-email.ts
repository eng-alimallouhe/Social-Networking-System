import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideCheckLine } from '@lucide/angular';
import { MfaManagementService } from '../../services/mfa-management.service';
import { finalize } from 'rxjs';
import { AppInput } from '../../../../../../shared/design-system/components/app-input/app-input';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';

@Component({
  selector: 'app-change-recovery-email',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    LucideCheckLine,
    RouterLink,
    AppInput
  ],
  templateUrl: './change-recovery-email.html',
  styleUrl: './change-recovery-email.css'
})
export class ChangeRecoveryEmail {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private mfaManagementService = inject(MfaManagementService);
  private loadingService = inject(GlobalLoaderService);

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

    this.mfaManagementService.initialRecoveryEmailChange({ newEmail })
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
