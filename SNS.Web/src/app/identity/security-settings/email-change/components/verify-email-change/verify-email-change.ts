import { Component, ElementRef, OnDestroy, QueryList, ViewChildren, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideChevronLeft, LucideCheckLine } from '@lucide/angular';
import { EmailChangeService } from '../../services/email-change.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-verify-email-change',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LucideChevronLeft,
    LucideCheckLine,
    RouterLink
  ],
  templateUrl: './verify-email-change.html',
  styleUrl: './verify-email-change.css',
})
export class VerifyEmailChange implements OnDestroy {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private emailChangeService = inject(EmailChangeService);
  private loadingService = inject(LoadingSettingsService);

  // Data received from InitialEmailChange via router state
  userId: string = history.state?.userId || '';
  token: string = history.state?.token || '';
  newEmail: string = history.state?.newEmail || '';

  // Resend countdown
  resendCooldown = signal(60);
  canResend = computed(() => this.resendCooldown() <= 0);
  private timerIntervalId?: ReturnType<typeof setInterval>;

  otpForm: FormGroup;

  @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef<HTMLInputElement>>;

  constructor() {
    this.otpForm = this.fb.group({
      code: this.fb.array(
        Array(6).fill(null).map(() =>
          this.fb.control('', [Validators.required, Validators.pattern('^[0-9]$')])
        )
      )
    });

    this.startResendTimer();
  }

  ngOnDestroy(): void {
    if (this.timerIntervalId) {
      clearInterval(this.timerIntervalId);
    }
  }

  get codeControls(): FormArray {
    return this.otpForm.get('code') as FormArray;
  }

  // ── OTP input handlers (same pattern as verify-account / verify-otp) ──

  onFocus(event: FocusEvent): void {
    (event.target as HTMLInputElement).select();
  }

  onInput(event: Event, index: number): void {
    const input = event.target as HTMLInputElement;
    const val = input.value.replace(/[^0-9]/g, '');
    input.value = val.length > 1 ? val.charAt(val.length - 1) : val;
    this.codeControls.at(index).setValue(input.value);
    if (input.value && index < 5) {
      this.codeInputs.toArray()[index + 1].nativeElement.focus();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number): void {
    const input = event.target as HTMLInputElement;
    if (event.key === 'Backspace') {
      if (!input.value && index > 0) {
        const prev = this.codeInputs.toArray()[index - 1];
        prev.nativeElement.value = '';
        this.codeControls.at(index - 1).setValue('');
        prev.nativeElement.focus();
      } else {
        input.value = '';
        this.codeControls.at(index).setValue('');
      }
    } else if (event.key === 'ArrowLeft' && index > 0) {
      this.codeInputs.toArray()[index - 1].nativeElement.focus();
    } else if (event.key === 'ArrowRight' && index < 5) {
      this.codeInputs.toArray()[index + 1].nativeElement.focus();
    }
  }

  onPaste(event: ClipboardEvent): void {
    event.preventDefault();
    const pasted = event.clipboardData?.getData('text/plain');
    if (!pasted) return;
    const nums = pasted.replace(/\D/g, '').substring(0, 6);
    for (let i = 0; i < nums.length; i++) {
      this.codeControls.at(i).setValue(nums[i]);
    }
    if (nums.length > 0) {
      this.codeInputs.toArray()[Math.min(nums.length, 5)].nativeElement.focus();
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
      code: this.codeControls.value.join('')
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
