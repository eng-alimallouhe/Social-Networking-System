import { Component, inject, signal, OnInit, effect } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideShield, LucideMail, LucideKey, LucideCheckCircle2, LucideXCircle, LucideChevronRight, LucideScanQrCode } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { MfaProvider } from '../../../../shared/contracts/mfa-provider.enum';
import { CommunicationMethod } from '../../../../shared/contracts/communication-method.enum';
import { AppConfirmDialog } from '../../../../../shared/components/app-confirm-dialog/app-confirm-dialog';
import { AppSelect, SelectOption } from '../../../../../shared/components/app-select/app-select';
import { map, finalize } from 'rxjs';

@Component({
  selector: 'app-user-security-settings',
  imports: [
    CommonModule,
    TranslatePipe,
    LucideShield,
    LucideMail,
    LucideKey,
    LucideCheckCircle2,
    LucideXCircle,
    LucideChevronRight,
    AppConfirmDialog,
    AppSelect,
    FormsModule,
    RouterLink,
    LucideScanQrCode
  ],
  templateUrl: './user-security-settings.html',
  styleUrl: './user-security-settings.css',
})
export class UserSecuritySettings implements OnInit {
  private securityService = inject(UserSecuritySettingsService);
  private loadingService = inject(LoadingSettingsService);
  private translateService = inject(TranslateService);
  private router = inject(Router);


  selectedMfaProvider = signal<MfaProvider | null>(null);
  selectedMethod = signal<CommunicationMethod | null>(null);

  MfaProvider = MfaProvider;
  CommunicationMethod = CommunicationMethod;

  showConfirmDisable = signal(false);
  isUpdatingProvider = signal(false);
  isUpdatingMethod = signal(false);

  providerOptions: SelectOption[] = [];
  communicationMethodOptions: SelectOption[] = [];

  securitySettingsResource = rxResource({
    stream: () => {
      this.loadingService.show();
      return this.securityService.getSecuritySettings().pipe(
        map(result => result.value),
        finalize(() => this.loadingService.hide())
      );
    }
  });


  constructor() {
    effect(() => {
      const settings = this.securitySettingsResource.value();
      if (settings) {
        this.selectedMfaProvider.set(settings.mfaProvider);
        this.selectedMethod.set(settings.defaultCommunicationMethod);
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.providerOptions = [
      { value: MfaProvider.None, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.0') },
      { value: MfaProvider.RecoveryEmail, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.1') },
      { value: MfaProvider.Email, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.2') },
      { value: MfaProvider.AuthenticatorApp, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.3') },
      { value: MfaProvider.Passkey, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.4') }
    ];

    this.communicationMethodOptions = [
      { value: CommunicationMethod.Email, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Methods.0') },
      { value: CommunicationMethod.RecoveryEmail, label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Methods.1') }
    ];
  }

  refreshSettings() {
    this.securitySettingsResource.reload();
  }

  onDisableMfaClick() {
    this.showConfirmDisable.set(true);
  }

  onCancelDisable() {
    this.showConfirmDisable.set(false);
  }

  onConfirmDisable() {
    this.showConfirmDisable.set(false);
    this.loadingService.show();
    this.securityService.disableMfa()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.refreshSettings();
          }
        }
      });
  }

  changeMfaProvider(newProvider: any) {
    const providerVal: MfaProvider = Number(newProvider);
    this.isUpdatingProvider.set(true);
    this.loadingService.show();

    const oldProvider = this.selectedMfaProvider();
    this.selectedMfaProvider.set(providerVal);

    this.securityService.changeMfaProvider({ newProvider: providerVal })
      .pipe(
        finalize(() => {
          this.isUpdatingProvider.set(false);
          this.loadingService.hide();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.refreshSettings();
          } else {
            this.selectedMfaProvider.set(oldProvider);
          }
        },
        error: (err) => {
          this.selectedMfaProvider.set(oldProvider);
        }
      });
  }

  changeDefaultCommunicationMethod(newMethod: any) {
    const methodVal: CommunicationMethod = Number(newMethod);
    this.isUpdatingMethod.set(true);
    this.loadingService.show();

    const oldMethod = this.selectedMethod();
    this.selectedMethod.set(methodVal);

    this.securityService.changeDefaultCommunicationMethod({ newCommunicationMethod: methodVal })
      .pipe(
        finalize(() => {
          this.isUpdatingMethod.set(false);
          this.loadingService.hide();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.refreshSettings();
          } else {
            this.selectedMethod.set(oldMethod);
          }
        },
        error: (err) => {
          this.selectedMethod.set(oldMethod);
        }
      });
  }

  navigateToLinkAuthenticator() {
    this.router.navigate(['/account-settings/security-settings/link-authenticator']);
  }

  navigateToChangeRecoveryEmail() {
    this.router.navigate(['/account-settings/security-settings/change-recovery-email']);
  }

  navigateToRecoveryCodes() {
    this.router.navigate(['/account-settings/security-settings/recovery-codes']);
  }

  navigateToPasskeys() {
    this.router.navigate(['/account-settings/security-settings/passkeys']);
  }
}
