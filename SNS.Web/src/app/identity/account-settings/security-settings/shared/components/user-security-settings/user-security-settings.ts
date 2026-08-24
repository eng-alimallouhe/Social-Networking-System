import { Component, inject, signal, OnInit, effect } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideChevronRight, LucideScanQrCode, LucideShieldCheck, LucideKey, LucidePencil, LucideExternalLink, LucideInfo } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { MfaProvider } from '../../../../../shared/contracts/mfa-provider.enum';
import { AppConfirmDialog } from '../../../../../../shared/design-system/components/app-confirm-dialog/app-confirm-dialog';
import { ConfirmStateService } from '../../../../../../shared/design-system/services/confirm-state.service';
import { ConfirmAction } from '../../../../../../shared/design-system/services/confirm-action.enum';
import { AppSelect, SelectOption } from '../../../../../../shared/design-system/components/app-select/app-select';
import { CommunicationMethod } from '../../../../../shared/contracts/communication-method.enum';
import { map, finalize, filter } from 'rxjs';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { MfaManagementService } from '../../../mfa-management/services/mfa-management.service';
import { SecuritySettingsStateService } from '../../services/security-settings-state.service';

@Component({
  selector: 'app-user-security-settings',
  imports: [
    CommonModule,
    TranslatePipe,
    LucideChevronRight,
    LucideShieldCheck,
    LucideKey,
    LucidePencil,
    LucideExternalLink,
    LucideInfo,
    AppConfirmDialog,
    AppSelect,
    FormsModule,
    LucideScanQrCode,
    RouterOutlet
],
  templateUrl: './user-security-settings.html',
  styleUrl: './user-security-settings.css',
})
export class UserSecuritySettings implements OnInit {
  private securityService = inject(UserSecuritySettingsService);
  private mfaManagementService = inject(MfaManagementService);
  private loadingService = inject(GlobalLoaderService);
  private translateService = inject(TranslateService);
  private router = inject(Router);
  private securitySettingsState = inject(SecuritySettingsStateService);
  private confirmStateService = inject(ConfirmStateService);
  private lastHandledChange = 0;

  ConfirmAction = ConfirmAction;

  selectedMfaProvider = signal<MfaProvider | null>(null);
  isRootRoute = signal(true);
  isUpdatingProvider = signal(false);
  showConfirmDisable = signal(false);
  showEnableMfaModal = signal(false);
  showCommunicationModal = signal(false);
  showRecoveryWarningModal = signal(false);
  isSettingsChanged = this.securitySettingsState.settingsChanged;

  MfaProvider = MfaProvider;
  CommunicationMethod = CommunicationMethod;

  providerOptions: SelectOption[] = [];

  securitySettingsResource = rxResource({
    stream: () => {
      this.loadingService.show();
      return this.securityService.getUserSecuritySettings().pipe(
        map(result => result.value),
        finalize(() => this.loadingService.hide())
      );
    }
  });


  constructor() {
    effect(() => {
      const action = this.confirmStateService.confirmedAction();
      if (action === ConfirmAction.Disable) {
        this.confirmStateService.consume();
        this.onConfirmDisable();
      }
    }, { allowSignalWrites: true });

    effect(() => {
      const settings = this.securitySettingsResource.value();

      if (settings) {
        this.selectedMfaProvider.set(
          settings.mfaProvider as unknown as MfaProvider
        );
      }
    });

    effect(() => {
      const isRoot = this.isRootRoute();
      const changeVersion = this.securitySettingsState.settingsChanged();

      if (
        isRoot &&
        changeVersion > 0 &&
        changeVersion !== this.lastHandledChange
      ) {
        this.lastHandledChange = changeVersion;
        this.securitySettingsResource.reload();
      }
    });

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.isRootRoute.set(event.urlAfterRedirects.split('?')[0].split('#')[0] === '/account-settings/security-settings');
    });
  }

  ngOnInit() {
    this.providerOptions = [
      { value: 'None', label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.0') },
      { value: 'RecoveryEmail', label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.1') },
      { value: 'Email', label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.2') },
      { value: 'AuthenticatorApp', label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.3') },
      { value: 'Passkey', label: this.translateService.instant('Identity.Security_Settings.Security_Settings_Page.Providers.4') }
    ];
  }

  refreshSettings() {
    this.securitySettingsResource.reload();
  }

  onDisableMfaClick() {
    this.showConfirmDisable.set(true);
  }

  onConfirmDisable() {
    this.loadingService.show();
    this.mfaManagementService.disableMfa()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.refreshSettings();
          }
        }
      });
  }

  enableMfaWithProvider(provider: MfaProvider) {
    this.showEnableMfaModal.set(false);
    this.loadingService.show();
    this.mfaManagementService.enableMfa({ mfaProvider: provider })
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
    const providerVal: any = newProvider;
    this.isUpdatingProvider.set(true);
    this.loadingService.show();

    const oldProvider = this.selectedMfaProvider();
    this.selectedMfaProvider.set(providerVal);

    this.mfaManagementService.changeMfaProvider({ newProvider: providerVal })
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

  onChangeCommunicationClick(settings: any) {
    if (!settings.recoveryEmail) {
      this.showRecoveryWarningModal.set(true);
    } else {
      this.showCommunicationModal.set(true);
    }
  }

  changeCommunicationMethod(method: CommunicationMethod) {
    this.showCommunicationModal.set(false);
    this.loadingService.show();
    this.mfaManagementService.changeDefaultCommunicationMethod({ newCommunicationMethod: method })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.refreshSettings();
          }
        }
      });
  }
}
