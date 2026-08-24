import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { LucideCheckLine } from '@lucide/angular';
import { MfaProvider } from '../../../../../shared/contracts/mfa-provider.enum';
import { finalize } from 'rxjs';
import { AppSelect, SelectOption } from '../../../../../../shared/design-system/components/app-select/app-select';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { MfaManagementService } from '../../services/mfa-management.service';

@Component({
  selector: 'app-enable-mfa',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LucideCheckLine,
    RouterLink,
    AppSelect
  ],
  templateUrl: './enable-mfa.html',
  styleUrl: './enable-mfa.css'
})
export class EnableMfa implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private mfaManagementService = inject(MfaManagementService);
  private loadingService = inject(GlobalLoaderService);
  private translateService = inject(TranslateService);

  form: FormGroup;
  providerOptions: SelectOption[] = [];

  constructor() {
    this.form = this.fb.group({
      provider: [null, [Validators.required]]
    });
  }

  ngOnInit() {
    this.providerOptions = [
      { value: MfaProvider.RecoveryEmail, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.1') },
      { value: MfaProvider.Email, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.2') },
      { value: MfaProvider.AuthenticatorApp, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.3') },
      { value: MfaProvider.Passkey, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.4') }
    ];
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    const selected: MfaProvider = this.form.value.provider;

    this.loadingService.show();
    this.mfaManagementService.enableMfa({ mfaProvider: selected })
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
