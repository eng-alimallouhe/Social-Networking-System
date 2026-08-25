import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { AppInput } from '../../../../../shared/design-system/components/app-input/app-input';
import { AppCheckbox } from '../../../../../shared/design-system/components/app-checkbox/app-checkbox';
import { GlobalLoaderService } from '../../../../../shared/Loading/services/global-loader.service';
import { ToastService } from '../../../../notifications/services/toast.service';
import { PasswordManagementService } from '../../services/password-management.service';
import { passwordMatchValidator } from '../../../../shared/validators/password-match.validator';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslatePipe,
    ReactiveFormsModule,
    AppInput,
    AppCheckbox
  ],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css'
})
export class ChangePassword {
  private fb = inject(FormBuilder);
  private passwordService = inject(PasswordManagementService);
  private loadingService = inject(GlobalLoaderService);
  private toastService = inject(ToastService);
  private translateService = inject(TranslateService);
  private router = inject(Router);

  isSubmitting = signal(false);

  form: FormGroup = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmNewPassword: ['', [Validators.required]],
    showPassword: [false]
  }, {
    validators: passwordMatchValidator('newPassword', 'confirmNewPassword')
  });

  get showPassword(): boolean {
    return this.form.get('showPassword')?.value;
  }

  onSubmit() {
    if (this.form.invalid || this.isSubmitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.loadingService.show();

    const request = {
      currentPassword: this.form.value.currentPassword,
      newPassword: this.form.value.newPassword
    };

    this.passwordService.changePassword(request)
      .pipe(finalize(() => {
        this.isSubmitting.set(false);
        this.loadingService.hide();
      }))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.toastService.success(
              this.translateService.instant("Status_Codes.Titles.Success"),
              this.translateService.instant("Identity.Security_Settings.Change_Password.Success_Message")
            );
            this.form.reset();
            this.router.navigate(['/account-settings/security-settings']);
          }
        },
        error: (err) => {
          // The global error interceptor or backend handles the 400 responses,
          // but we can add specific handling if required.
        }
      });
  }
}
