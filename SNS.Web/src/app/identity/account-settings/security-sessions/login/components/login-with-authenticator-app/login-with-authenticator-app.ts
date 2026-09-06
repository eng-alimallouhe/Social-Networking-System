import { Component, ElementRef, OnInit, QueryList, ViewChildren, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { loginWithAuthenticatorRequest } from '../../contracts/login-with-authenticator-request.dto';
import { LoginService } from '../../services/login.service';
import { finalize } from 'rxjs';
import { AuthenticationService } from '../../../../../shared/services/authentication.service';
import { RequestInformationService } from '../../../../../shared/services/request-information.service';
import { LucideScanEye, LucideCircleCheckBig } from "@lucide/angular";
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { AppCodeInput } from '../../../../../../shared/design-system/components/app-code-input/app-code-input';

@Component({
  selector: 'app-login-with-authenticator-app',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    TranslatePipe,
    LucideScanEye,
    LucideCircleCheckBig,
    AppCodeInput
  ],
  templateUrl: './login-with-authenticator-app.html',
  styleUrl: './login-with-authenticator-app.css',
})
export class LoginWithAuthenticatorApp implements OnInit {
  private loginService = inject(LoginService);
  private loadingService = inject(GlobalLoaderService);
  private authenticationService = inject(AuthenticationService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private requestInformationService = inject(RequestInformationService);

  authenticatorForm: FormGroup;
  isLoading = this.loadingService.isLoading;
  ui: string = '';

  constructor() {
    this.authenticatorForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['ui']) {
        this.ui = params['ui'];
      }
    });
  }



  onSubmit() {
    if (this.authenticatorForm.invalid) {
      this.authenticatorForm.markAllAsTouched();
      return;
    }

    const loginWithAuthenticatorRequest: loginWithAuthenticatorRequest = {
      userIdentifier: this.ui,
      code: this.authenticatorForm.value.code
    }

    this.loadingService.show();
    this.loginService.loginWithAuthenticator(loginWithAuthenticatorRequest)
      .pipe(
        finalize(() => {
          this.loadingService.hide();
        })
      ).subscribe({
        next: (response) => {
          this.authenticationService.setAccessToken(response.value?.accessToken!);
          this.requestInformationService.setDeviceId(response.value?.deviceId!);
          this.router.navigate(['/']);
        },
        error: (err) => {
          this.authenticatorForm.get('code')?.reset();
        }
      });
  }
}