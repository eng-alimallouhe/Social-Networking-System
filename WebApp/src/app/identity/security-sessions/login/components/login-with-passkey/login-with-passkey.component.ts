import { Component, inject, OnInit } from '@angular/core';
import { LoginService } from '../../services/login.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, forkJoin } from 'rxjs';
import { Result } from '../../../../../shared/contracts/result';
import { RequestInformationService } from '../../../../shared/services/request-information.service';
import { TokenService } from '../../../../shared/services/token.service';
import { LoginResponse } from '../../contracts/login-response.dto';
import { RouterLink } from '@angular/router';
import { LinearLoaderComponent } from '../../../../../shared/components/loaders/linear-loader/linear-loader.component';

@Component({
  selector: 'app-login-with-passkey',
  imports: [TranslatePipe, RouterLink, LinearLoaderComponent],
  templateUrl: './login-with-passkey.component.html',
  styleUrl: './login-with-passkey.component.css',
})
export class LoginWithPasskeyComponent implements OnInit {
  private loginService = inject(LoginService);
  private toastService = inject(ToastService);
  private route = inject(ActivatedRoute);
  private translateService = inject(TranslateService);
  private tokenService = inject(TokenService);
  private requestInformationService = inject(RequestInformationService);
  private router = inject(Router);

  isLoadingResponse = false;

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
    this.isLoadingResponse = true;
    this.loginService.initiateLogin(identifier)
      .pipe(finalize(() => {
        this.isLoadingResponse = false;
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

            // الخطوة 2: استدعاء المتصفح (نافذة البصمة/الوجه)
            const assertion = await navigator.credentials.get({
              publicKey: publicKeyOptions
            }) as PublicKeyCredential;

            // الخطوة 3: تجهيز الرد للباك إند
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

            this.isLoadingResponse = true;
            this.loginService.completeLogin(payload)
              .pipe(finalize(() => {
                this.isLoadingResponse = false;
              }))
              .subscribe({
                next: (response) => {
                  this.tokenService.setToken(response.value?.accessToken!, response.value?.refreshToken!);
                  this.requestInformationService.setDeviceId(response.value?.deviceId!);
                  this.router.navigate(['/']);
                },
                error: (err) => {
                  var errorResult = err.error as Result<LoginResponse>;

                  if (errorResult && errorResult.statusCode) {
                    let category = errorResult.statusCode.category;
                    let code = errorResult.statusCode.code;

                    forkJoin({
                      message: this.translateService.get(`Status_Codes.${category}.${code}`),
                      title: this.translateService.get(`Status_Codes.Shared.Error_Title`)
                    }).subscribe(translations => {
                      this.toastService.error(translations.title, translations.message, 5000);
                    });
                  }
                }
              });

          } catch (err) {
            this.isLoadingResponse = false;
            this.toastService.error("The operation was cancelled or a documentation error occurred", "");
          }
        },
        error: () => this.toastService.error("No registered keys were found for this user", "")
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