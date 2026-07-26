import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslateModule, TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LoginService } from '../../services/login.service';
import { LoginWithPasswordRequest } from '../../contracts/login-with-password-request.dto';
import { finalize, forkJoin } from 'rxjs';
import { LinearLoaderComponent } from '../../../../../shared/components/loaders/linear-loader/linear-loader.component';
import { ToastService } from '../../../../../shared/services/toast.service';
import { MfaProvider } from '../../../../shared/contracts/mfa-provider.enum';
import { AuthFlowService } from '../../../../shared/services/auth-flow.service';
import { RequestInformationService } from '../../../../shared/services/request-information.service';
import { TokenService } from '../../../../shared/services/token.service';
import { Result } from '../../../../../shared/contracts/result';
import { LoginResponse } from '../../contracts/login-response.dto';
import { LucideAArrowDown, LucideCheck, LucideLockKeyhole, LucideSquareUser, LucideSquareUserRound } from "@lucide/angular";

@Component({
  selector: 'app-login-with-password',
  imports: [
    RouterLink,
    CommonModule,
    TranslatePipe,
    ReactiveFormsModule,
    LinearLoaderComponent,
    LucideSquareUserRound,
    LucideLockKeyhole,
    LucideCheck
  ],
  templateUrl: './login-with-password.component.html',
  styleUrl: './login-with-password.component.css',
})
export class LoginWithPasswordComponent {
  private fb = inject(FormBuilder);
  private loginService = inject(LoginService);
  private toastService = inject(ToastService);
  private authFlowService = inject(AuthFlowService);
  private router = inject(Router);
  private translateService = inject(TranslateService);
  private tokenService = inject(TokenService);
  private requestInformationService = inject(RequestInformationService);

  public isLoadingResponse = false;


  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    remember: [false]
  });

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;

      let sendData: LoginWithPasswordRequest = {
        identifier: loginData.email,
        password: loginData.password,
      }

      this.isLoadingResponse = true;

      this.loginService.loginWithPassword(sendData)
        .pipe(finalize(() => {
          this.isLoadingResponse = false;
        }))
        .subscribe({
          next: (response) => {
            var loginResponse = response.value;

            if (loginResponse?.isMfaRequired) {
              if (loginResponse.mfaProviderType == MfaProvider.AuthenticatorApp) {
                this.router.navigate(['/auth/login-with-authenticator-app'], { queryParams: { ui: sendData.identifier } });
              } else if (loginResponse.mfaProviderType == MfaProvider.Email || loginResponse.mfaProviderType == MfaProvider.RecoveryEmail) {
                this.authFlowService.setTfaRecipientAddress(sendData.identifier);
                this.router.navigate(['/auth/verify-otp'], { queryParams: { uid: loginResponse.userId, "challenge-token": loginResponse.challengeToken } });
              } else {
                this.router.navigate(['/auth/login-with-passkey'], { queryParams: { ui: sendData.identifier } });
              }

            }

            this.tokenService.setToken(response.value?.accessToken!, response.value?.refreshToken!);
            this.requestInformationService.setDeviceId(response.value?.deviceId!);
            this.router.navigate(['/']);

            this.toastService.success('Login Success', 'you will be redirected in 5 secounds', 5000);
            this.router.navigate(['/home']);
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
        })

    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}