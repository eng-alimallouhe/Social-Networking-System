import { Component, ElementRef, QueryList, ViewChildren, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideCheckLine } from '@lucide/angular';
import { finalize } from 'rxjs';
import { AppCodeInput } from '../../../../../../shared/design-system/components/app-code-input/app-code-input';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { MfaManagementService } from '../../services/mfa-management.service';

@Component({
  selector: 'app-verify-authenticator',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LucideCheckLine,
    RouterLink,
    AppCodeInput
  ],
  templateUrl: './verify-authenticator.html',
  styleUrl: './verify-authenticator.css'
})
export class VerifyAuthenticator {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private mfaManagementService = inject(MfaManagementService);
  private loadingService = inject(GlobalLoaderService);

  otpForm: FormGroup;

  constructor() {
    this.otpForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
    });
  }



  onSubmit(): void {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    this.loadingService.show();
    const code = this.otpForm.value.code;

    this.mfaManagementService.completeAuthenticatorRegistration({ code })
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
