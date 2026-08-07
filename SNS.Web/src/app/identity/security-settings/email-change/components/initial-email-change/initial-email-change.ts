import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LucideChevronLeft, LucideCheckLine } from '@lucide/angular';
import { EmailChangeService } from '../../services/email-change.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-initial-email-change',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    LucideChevronLeft,
    LucideCheckLine,
    RouterLink
  ],
  templateUrl: './initial-email-change.html',
  styleUrl: './initial-email-change.css',
})
export class InitialEmailChange {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private emailChangeService = inject(EmailChangeService);
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

    this.emailChangeService.initialEmailChange({ newEmail })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (result) => {
          if (result.isSuccess && result.value) {
            this.router.navigate(
              ['/account-settings/personal-information/verify-email-change'],
              {
                state: {
                  userId: result.value.userId,
                  token: result.value.token,
                  newEmail: newEmail
                }
              }
            );
          }
        }
      });
  }
}
