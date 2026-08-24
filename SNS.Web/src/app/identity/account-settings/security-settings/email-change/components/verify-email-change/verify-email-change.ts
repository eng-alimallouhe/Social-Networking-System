import { Component, ElementRef, OnDestroy, QueryList, ViewChildren, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideCheckLine } from '@lucide/angular';
import { EmailChangeService } from '../../services/email-change.service';
import { finalize } from 'rxjs';
import { AppCodeInput } from '../../../../../../shared/design-system/components/app-code-input/app-code-input';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';

@Component({
  selector: 'app-verify-email-change',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LucideCheckLine,
    RouterLink,
    AppCodeInput
  ],
  templateUrl: './verify-email-change.html',
  styleUrl: './verify-email-change.css',
})
export class VerifyEmailChange implements OnDestroy {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private emailChangeService = inject(EmailChangeService);
  private loadingService = inject(GlobalLoaderService);

  // Data received from InitialEmailChange via router state
  userId: string = history.state?.userId || '';
  token: string = history.state?.token || '';
  newEmail: string = history.state?.newEmail || '';

  // Resend countdown
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



  // ── Submit ──

  onSubmit(): void {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    this.loadingService.show();

    this.emailChangeService.confirmEmailChange({
      userId: this.userId,
      token: this.token,
      code: this.otpForm.value.code
    })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: () => {
          this.router.navigate(['/account-settings/personal-information']);
        }
      });
  }

  // ── Resend ──

  resendCode(): void {
    if (!this.canResend()) return;

    this.loadingService.show();
    this.emailChangeService.resendEmailChangeCode()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: () => {
          this.startResendTimer();
        }
      });
  }

  // ── Timer ──

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
