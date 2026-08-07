import { Component, ElementRef, QueryList, ViewChildren, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideChevronLeft, LucideCheckLine } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-verify-authenticator',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LucideChevronLeft,
    LucideCheckLine,
    RouterLink
  ],
  templateUrl: './verify-authenticator.html',
  styleUrl: './verify-authenticator.css'
})
export class VerifyAuthenticator {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private securityService = inject(UserSecuritySettingsService);
  private loadingService = inject(LoadingSettingsService);

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
  }

  get codeControls(): FormArray {
    return this.otpForm.get('code') as FormArray;
  }

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

  onSubmit(): void {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    this.loadingService.show();
    const code = this.codeControls.value.join('');

    this.securityService.completeAuthenticatorRegistration({ code })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.router.navigate(['/account-settings/security-settings']);
          }
        }
      });
  }
}
