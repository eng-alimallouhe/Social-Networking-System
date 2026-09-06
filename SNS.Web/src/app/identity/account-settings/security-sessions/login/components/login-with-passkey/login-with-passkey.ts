import { Component, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { ToastService } from '../../../../../notifications/services/toast.service';
import { RequestInformationService } from '../../../../../shared/services/request-information.service';
import { AuthenticationService } from '../../../../../shared/services/authentication.service';
import { LoginService } from '../../services/login.service';
import { LucideFingerprintPattern } from "@lucide/angular";
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';

@Component({
  selector: 'app-login-with-passkey',
  imports: [TranslatePipe, RouterLink, LucideFingerprintPattern],
  templateUrl: './login-with-passkey.html',
  styleUrl: './login-with-passkey.css',
})
export class LoginWithPasskey {
  private loginService = inject(LoginService);
  private toastService = inject(ToastService);
  private route = inject(ActivatedRoute);
  private authenticationService = inject(AuthenticationService);
  private requestInformationService = inject(RequestInformationService);
  private router = inject(Router);
  private loadingService = inject(GlobalLoaderService);

  isLoading = this.loadingService.isLoading;

  ui: string = '';

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['ui']) {
        this.ui = params['ui'];
      }
    });

    if (this.ui) {
      this.onPasskeyLogin(this.ui);
    }
  }

  async onPasskeyLogin(identifier: string) {
    this.loadingService.show();
    this.loginService.initiateLogin(identifier)
      .pipe(finalize(() => {
        this.loadingService.hide();
      }))
      .subscribe({
        next: async (options) => {
          try {
            const publicKeyOptions = {
              ...options,
              challenge: base64ToBuffer(options.challenge),
              allowCredentials: options.allowCredentials.map((c: any) => ({
                ...c,
                id: base64ToBuffer(c.id)
              }))
            };

            const assertion = await navigator.credentials.get({
              publicKey: publicKeyOptions
            }) as PublicKeyCredential;

            const response = assertion.response as AuthenticatorAssertionResponse;
            const payload = {
              userId: options.userId,
              assertionResponse: {
                id: assertion.id,
                rawId: bufferToBase64(assertion.rawId),
                type: assertion.type,
                response: {
                  authenticatorData: bufferToBase64(response.authenticatorData),
                  clientDataJSON: bufferToBase64(response.clientDataJSON),
                  signature: bufferToBase64(response.signature),
                  userHandle: response.userHandle ? bufferToBase64(response.userHandle) : null
                }
              }
            };

            this.loadingService.show();
            this.loginService.completeLogin(payload)
              .pipe(finalize(() => {
                this.loadingService.hide();
              }))
              .subscribe({
                next: (response) => {
                  this.authenticationService.setAccessToken(response.value?.accessToken!);
                  this.requestInformationService.setDeviceId(response.value?.deviceId!);
                  this.router.navigate(['/']);
                }
              });

          } catch (err) {
            this.loadingService.hide();
            this.toastService.error("The operation was cancelled or a documentation error occurred", "");
          }
        }
      });
  }
}

export const bufferToBase64 = (buffer: ArrayBuffer): string => {
  return btoa(String.fromCharCode(...new Uint8Array(buffer)));
};

export const base64ToBuffer = (base64: string): ArrayBuffer => {
  const binary = atob(base64);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
  return bytes.buffer;
};