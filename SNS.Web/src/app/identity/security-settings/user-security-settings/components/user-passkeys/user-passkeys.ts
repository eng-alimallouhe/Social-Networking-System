import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { LucideChevronLeft, LucidePlus, LucideTrash2, LucideKeyRound, LucideLaptop } from '@lucide/angular';
import { UserSecuritySettingsService } from '../../services/user-security-settings.service';
import { LoadingSettingsService } from '../../../shared/services/loading-settings.service';
import { PasskeyDto } from '../../contracts/passkey.dto';
import { AppConfirmDialog } from '../../../../../shared/components/app-confirm-dialog/app-confirm-dialog';
import { DeviceNameDialog } from './device-name-dialog/device-name-dialog';
import { finalize } from 'rxjs';
import { WebAuthnUtils } from '../../../../../shared/utils/webauthn-utils';
import { ToastService } from '../../../../notifications/services/toast.service';

@Component({
  selector: 'app-user-passkeys',
  imports: [
    CommonModule,
    TranslatePipe,
    LucidePlus,
    LucideTrash2,
    LucideKeyRound,
    RouterLink,
    AppConfirmDialog,
    DeviceNameDialog
  ],
  templateUrl: './user-passkeys.html',
  styleUrl: './user-passkeys.css'
})
export class UserPasskeys {
  private securityService = inject(UserSecuritySettingsService);
  private loadingService = inject(LoadingSettingsService);
  private toastService = inject(ToastService);

  passkeysList = signal<PasskeyDto[]>([]);
  hasPasskeys = computed(() => this.passkeysList().length > 0);

  showDeleteConfirm = signal(false);
  passkeyToDelete = signal<string | null>(null);

  isRegistering = signal(false);
  isWebAuthnSupported = signal(true);

  showDeviceNameDialog = signal(false);
  pendingAttestationResponse = signal<any>(null);

  constructor() {
    this.checkWebAuthnSupport();
    this.loadPasskeys();
  }

  checkWebAuthnSupport() {
    if (!window.PublicKeyCredential) {
      this.isWebAuthnSupported.set(false);
    }
  }

  loadPasskeys() {
    this.loadingService.show();
    this.securityService.getUserPasskeys()
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess && res.value) {
            this.passkeysList.set(res.value);
          }
        }
      });
  }

  onAddPasskey() {
    if (!this.isWebAuthnSupported()) return;

    this.isRegistering.set(true);
    this.loadingService.show();

    this.securityService.initiatePasskeyRegistration({ attestationType: 'none' })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: async (res) => {
          if (res.isSuccess && res.value) {
            try {
              const options = res.value;

              const publicKeyCredentialCreationOptions: PublicKeyCredentialCreationOptions = {
                rp: options.rp,
                user: {
                  id: WebAuthnUtils.base64urlToBuffer(options.user.id),
                  name: options.user.name,
                  displayName: options.user.displayName
                },
                challenge: WebAuthnUtils.base64urlToBuffer(options.challenge),
                pubKeyCredParams: options.pubKeyCredParams as PublicKeyCredentialParameters[],
                timeout: options.timeout,
                attestation: options.attestation as AttestationConveyancePreference,
                authenticatorSelection: options.authenticatorSelection as AuthenticatorSelectionCriteria,
                excludeCredentials: options.excludeCredentials?.map(c => ({
                  id: WebAuthnUtils.base64urlToBuffer(c.id),
                  type: c.type as PublicKeyCredentialType
                }))
              };

              const credential = await navigator.credentials.create({ publicKey: publicKeyCredentialCreationOptions }) as PublicKeyCredential;

              const response = credential.response as AuthenticatorAttestationResponse;

              const attestationResponse = {
                id: credential.id,
                rawId: WebAuthnUtils.bufferToBase64url(credential.rawId),
                type: credential.type,
                response: {
                  attestationObject: WebAuthnUtils.bufferToBase64url(response.attestationObject),
                  clientDataJSON: WebAuthnUtils.bufferToBase64url(response.clientDataJSON),
                  transports: typeof response.getTransports === 'function' ? response.getTransports() : []
                }
              };

              this.pendingAttestationResponse.set(attestationResponse);
              this.showDeviceNameDialog.set(true);
            } catch (err: any) {
              if (err.name !== 'NotAllowedError' && err.name !== 'AbortError') {
                this.toastService.error('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationFailed', err.message || '');
              }
            } finally {
              this.isRegistering.set(false);
            }
          } else {
            this.isRegistering.set(false);
            this.toastService.error('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationFailed', 'Invalid response');
          }
        },
        error: () => {
          this.isRegistering.set(false);
          this.toastService.error('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationFailed', 'Network error');
        }
      });
  }

  onDeviceNameConfirm(deviceName: string) {
    this.showDeviceNameDialog.set(false);
    const attestation = this.pendingAttestationResponse();

    if (!attestation) return;

    this.loadingService.show();
    this.securityService.completePasskeyRegistration({
      attestationResponse: attestation,
      deviceName: deviceName
    })
      .pipe(finalize(() => {
        this.loadingService.hide();
        this.pendingAttestationResponse.set(null);
      }))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.toastService.success('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationSuccess', 'Success');
            this.loadPasskeys();
          } else {
            this.toastService.error('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationFailed', 'Registration failed');
          }
        },
        error: () => {
          this.toastService.error('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationFailed', 'Network error');
        }
      });
  }

  onDeviceNameCancel() {
    this.showDeviceNameDialog.set(false);
    this.pendingAttestationResponse.set(null);
  }

  onDeleteClick(passkeyId: string) {
    this.passkeyToDelete.set(passkeyId);
    this.showDeleteConfirm.set(true);
  }

  onCancelDelete() {
    this.passkeyToDelete.set(null);
    this.showDeleteConfirm.set(false);
  }

  onConfirmDelete() {
    const id = this.passkeyToDelete();
    if (!id) return;

    this.showDeleteConfirm.set(false);
    this.loadingService.show();

    this.securityService.removePasskey({ passkeyId: id })
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.loadPasskeys();
          }
        }
      });
  }
}
