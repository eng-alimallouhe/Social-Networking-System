import { Component, ElementRef, OnDestroy, QueryList, ViewChildren, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideCheckLine } from '@lucide/angular';
import { finalize } from 'rxjs';
import { AppCodeInput } from '../../../../../../shared/design-system/components/app-code-input/app-code-input';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { MfaManagementService } from '../../services/mfa-management.service';

@Component({
  selector: 'app-verify-recovery-email',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LucideCheckLine,
    RouterLink,
    AppCodeInput
  ],
  templateUrl: './verify-recovery-email.html',
  styleUrl: './verify-recovery-email.css'
})
export class VerifyRecoveryEmail implements OnDestroy {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private mfaManagementService = inject(MfaManagementService);
  private loadingService = inject(GlobalLoaderService);

  userId: string = history.state?.userId || '';
  token: string = history.state?.token || '';
  newEmail: string = history.state?.newEmail || '';

  resendCooldown = signal(60);
  canResend = computed(() => this.resendCooldown() <= 0);
  private timerIntervalId?: ReturnType<typeof setInterval>;

  otpForm: FormGroup;

  constructor() {
    this.otpForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
    });

    this.startResendTimer();
  }

  ngOnDestroy(): void {
    if (this.timerIntervalId) {
      clearInterval(this.timerIntervalId);
    }
  }



  onSubmit(): void {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    this.loadingService.show();
    this.mfaManagementService.verifyRecoveryEmailChange({
      userId: this.userId,
      token: this.token,
      code: this.otpForm.value.code
    })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.router.navigate(['/account-settings/security-settings']);
          }
        }
      });
  }

  resendCode(): void {
    if (!this.canResend()) return;

    this.loadingService.show();
    this.mfaManagementService.resendRecoveryEmailChangeCode({ token: this.token })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            // Update token if returned new one
            this.token = res.value.token;
            this.startResendTimer();
          }
        }
      });
  }

  private startResendTimer(): void {
    if (this.timerIntervalId) {
      clearInterval(this.timerIntervalId);
    }
    this.resendCooldown.set(60);
    this.timerIntervalId = setInterval(() => {
      this.resendCooldown.update(val => val - 1);
      if (this.resendCooldown() <= 0) {
        clearInterval(this.timerIntervalId);
      }
    }, 1000);
  }
}
