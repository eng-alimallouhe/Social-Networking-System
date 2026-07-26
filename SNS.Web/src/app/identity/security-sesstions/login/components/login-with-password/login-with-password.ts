import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize, forkJoin } from 'rxjs';
import { Result } from '../../../../../shared/contracts/result';
import { MfaProvider } from '../../../../shared/contracts/mfa-provider.enum';
import { LoginResponse } from '../../contracts/login-response.dto';
import { LoginWithPasswordRequest } from '../../contracts/login-with-password-request.dto';
import { LoginService } from '../../services/login.service';
import { ToastService } from '../../../../notifications/services/toast.service';
import { AuthFlowService } from '../../../../shared/services/auth-flow.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TokenService } from '../../../../shared/services/token.service';
import { RequestInformationService } from '../../../../shared/services/request-information.service';
import { LucideCheck, LucideLockKeyhole, LucideSquareUserRound, LucideLogIn } from '@lucide/angular';
import { LoadingAuthService } from '../../../../shared/layout/services/loading-auth.service';

@Component({
  selector: 'app-login-with-password',
  imports: [
    RouterLink,
    CommonModule,
    TranslatePipe,
    ReactiveFormsModule,
    LucideSquareUserRound,
    LucideLockKeyhole,
    LucideCheck,
    LucideLogIn
  ],
  templateUrl: './login-with-password.html',
  styleUrl: './login-with-password.css',
})
export class LoginWithPassword implements OnInit {
  private fb = inject(FormBuilder);
  private loginService = inject(LoginService);
  private toastService = inject(ToastService);
  private authFlowService = inject(AuthFlowService);
  private router = inject(Router);
  private loadingService = inject(LoadingAuthService);
  private translateService = inject(TranslateService);
  private tokenService = inject(TokenService);
  private requestInformationService = inject(RequestInformationService);

  showPassword = signal(false);
  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required]],
    password: ['', [Validators.required]],
    remember: [true],
    showPassword: [false]
  });

  toggleShowPassword() {
    this.showPassword.update(value => !value);
  }

  ngOnInit(): void {
    this.loginForm.get('showPassword')?.valueChanges.subscribe(value => {
      this.showPassword.set(value);
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;
      let sendData: LoginWithPasswordRequest = {
        identifier: loginData.email,
        password: loginData.password,
      }

      this.loadingService.show();

      this.loginService.loginWithPassword(sendData)
        .pipe(finalize(() => {
          this.loadingService.hide();
        }))
        .subscribe({
          next: (response: Result<LoginResponse>) => {
            var loginResponse = response.value;

            if (loginResponse?.isMfaRequired) {
              switch (loginResponse.mfaProviderType) {
                case MfaProvider.AuthenticatorApp:
                  this.router.navigate(['/auth/login-with-authenticator-app'], { queryParams: { ui: sendData.identifier } });
                  break;
                case MfaProvider.Email || MfaProvider.RecoveryEmail:
                  this.authFlowService.setTfaRecipientAddress(sendData.identifier);
                  this.router.navigate(['/auth/verify-otp'], { queryParams: { uid: loginResponse.userId, "challenge-token": loginResponse.challengeToken } });
                  break;
                case MfaProvider.Passkey:
                  this.router.navigate(['/auth/login-with-passkey'], { queryParams: { ui: sendData.identifier } });
                  break;
              }
            }

            this.tokenService.setToken(response.value?.accessToken!, response.value?.refreshToken!);
            this.requestInformationService.setDeviceId(response.value?.deviceId!);

            this.toastService.success('Login Success', 'you will be redirected in 5 secounds', 5000);
            this.router.navigate(['/']);
          },
          error: (err) => {
            var errorResult = err.error as Result<LoginResponse>;

            if (errorResult && errorResult.statusCode) {
              let category = errorResult.statusCode.category;
              let code = errorResult.statusCode.code;

              if (category === 'User' && code === 404) {
                forkJoin({
                  message: this.translateService.get(`Identity.Security_Sessions.Login.Login_Error_Body`),
                  title: this.translateService.get(`Identity.Security_Sessions.Login.Login_Error_Title`)
                }).subscribe(translations => {
                  this.toastService.error(translations.title, translations.message, 5000);
                });
              }
              else {
                forkJoin({
                  message: this.translateService.get(`Status_Codes.${category}.${code}`),
                  title: this.translateService.get(`Status_Codes.Shared.Error_Title`)
                }).subscribe(translations => {
                  this.toastService.error(translations.title, translations.message, 5000);
                });
              }
            }
          }
        })
    } else {
      this.loginForm.markAllAsTouched();
      console.log("خيو صحصح شبك ؟؟!!!");

    }
  }
}
