  import { Component, inject, signal, computed } from '@angular/core';
  import { CommonModule } from '@angular/common';
  import { TranslatePipe, TranslateService } from '@ngx-translate/core';
  import { RouterLink } from '@angular/router';
  import { LucidePlus, LucideTrash2, LucideKeyRound } from '@lucide/angular';
  import { PasskeyDto } from '../../contracts/passkey.dto';
  import { AppConfirmDialog } from '../../../../../../shared/design-system/components/app-confirm-dialog/app-confirm-dialog';
  import { ConfirmStateService } from '../../../../../../shared/design-system/services/confirm-state.service';
  import { ConfirmAction } from '../../../../../../shared/design-system/services/confirm-action.enum';
  import { DeviceNameDialog } from './device-name-dialog/device-name-dialog';
  import { effect } from '@angular/core';
  import { finalize } from 'rxjs';
  import { WebAuthnUtils } from '../../../../../../shared/utils/webauthn-utils';
  import { ToastService } from '../../../../../notifications/services/toast.service';
  import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
  import { MfaManagementService } from '../../services/mfa-management.service';

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
    private mfaManagementService = inject(MfaManagementService);
    private loadingService = inject(GlobalLoaderService);
    private toastService = inject(ToastService);
    private translate = inject(TranslateService);
    private confirmStateService = inject(ConfirmStateService);

    ConfirmAction = ConfirmAction;

    passkeysList = signal<PasskeyDto[]>([]);
    hasPasskeys = computed(() => this.passkeysList().length > 0);

    showDeleteConfirm = signal(false);
    passkeyToDelete = signal<string | null>(null);

    isRegistering = signal(false);
    isWebAuthnSupported = signal(true);

    showDeviceNameDialog = signal(false);
    pendingAttestationResponse = signal<any>(null);

    constructor() {
      effect(() => {
        const action = this.confirmStateService.confirmedAction();
        if (action === ConfirmAction.Delete) {
          this.confirmStateService.consume();
          this.onConfirmDelete();
        }
      }, { allowSignalWrites: true });

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
      this.mfaManagementService.getUserPasskeys()
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

      this.mfaManagementService.initiatePasskeyRegistration({ attestationType: 'none' })
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
                  clientExtensionResults: credential.getClientExtensionResults ? credential.getClientExtensionResults() : {},
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
                  var title = this.translate.instant('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationFailed');
                  this.toastService.error(title, err.message || '');
                }
              } finally {
                this.isRegistering.set(false);
              }
            } else {
              this.isRegistering.set(false);
              const title = this.translate.instant('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.RegistrationFailed');
              const message = this.translate.instant('Identity.Security_Settings.Security_Settings_Page.Passkeys_Page.InvalidResponse');
              this.toastService.error(title, message);
            }
          },
          error: () => {
            this.isRegistering.set(false);
          }
        });
    }

    onDeviceNameConfirm(deviceName: string) {
      this.showDeviceNameDialog.set(false);
      const attestation = this.pendingAttestationResponse();

      if (!attestation) return;

      this.loadingService.show();
      this.mfaManagementService.completePasskeyRegistration({
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
            }
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

    onConfirmDelete() {
      const id = this.passkeyToDelete();
      if (!id) return;

      this.showDeleteConfirm.set(false);
      this.loadingService.show();

      this.mfaManagementService.removePasskey({ passkeyId: id })
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
