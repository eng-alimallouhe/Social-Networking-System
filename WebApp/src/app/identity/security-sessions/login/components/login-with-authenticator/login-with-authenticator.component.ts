import { Component, ElementRef, OnInit, QueryList, ViewChildren, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { LinearLoaderComponent } from '../../../../../shared/components/loaders/linear-loader/linear-loader.component';
import { AuthFlowService } from '../../../../shared/services/auth-flow.service';
import { loginWithAuthenticatorRequest } from '../../contracts/login-with-authenticator-request.dto';
import { LoginService } from '../../services/login.service';
import { finalize, forkJoin } from 'rxjs';
import { TokenService } from '../../../../shared/services/token.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { Result } from '../../../../../shared/contracts/result';
import { LoginResponse } from '../../contracts/login-response.dto';
import { RequestInformationService } from '../../../../shared/services/request-information.service';

@Component({
  selector: 'app-login-with-authenticator',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslateModule, LinearLoaderComponent],
  templateUrl: './login-with-authenticator.component.html',
  styleUrl: './login-with-authenticator.component.css'
})
export class LoginWithAuthenticatorComponent implements OnInit {
  private loginService = inject(LoginService);
  private tokenService = inject(TokenService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private toatsService = inject(ToastService);
  private translateService = inject(TranslateService);
  private requestInformationService = inject(RequestInformationService);

  authenticatorForm: FormGroup;
  ui: string = '';
  isLoadingResponse = false;

  @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef<HTMLInputElement>>;

  constructor() {
    this.authenticatorForm = this.fb.group({
      code: this.fb.array(
        Array(6).fill(null).map(() => this.fb.control('', [Validators.required, Validators.pattern('^[0-9]$')]))
      )
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['ui']) {
        this.ui = params['ui'];
      }
    });
  }

  get codeControls() {
    return this.authenticatorForm.get('code') as FormArray;
  }

  onFocus(event: FocusEvent) {
    const input = event.target as HTMLInputElement;
    input.select();
  }

  onInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    const val = input.value.replace(/[^0-9]/g, '');
    input.value = val.length > 1 ? val.charAt(val.length - 1) : val;

    this.codeControls.at(index).setValue(input.value);

    if (input.value && index < 5) {
      this.codeInputs.toArray()[index + 1].nativeElement.focus();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;

    if (event.key === 'Backspace') {
      if (!input.value && index > 0) {
        const prevInput = this.codeInputs.toArray()[index - 1];
        prevInput.nativeElement.value = '';
        this.codeControls.at(index - 1).setValue('');
        prevInput.nativeElement.focus();
      } else {
        input.value = '';
        this.codeControls.at(index).setValue('');
      }
    } else if (event.key === 'ArrowLeft' && index > 0) {
      this.codeInputs.toArray()[index - 1].nativeElement.focus();
    } else if (event.key === 'ArrowRight' && index < 5) {
      this.codeInputs.toArray()[index + 1].nativeElement.focus();
    }
  }

  onPaste(event: ClipboardEvent) {
    event.preventDefault();
    const pastedData = event.clipboardData?.getData('text/plain');
    if (!pastedData) return;

    const numbersOnly = pastedData.replace(/\D/g, '').substring(0, 6);

    for (let i = 0; i < numbersOnly.length; i++) {
      this.codeControls.at(i).setValue(numbersOnly[i]);
    }

    if (numbersOnly.length > 0) {
      const focusIndex = Math.min(numbersOnly.length, 5);
      this.codeInputs.toArray()[focusIndex].nativeElement.focus();
    }
  }

  onSubmit() {
    if (this.authenticatorForm.invalid) {
      this.authenticatorForm.markAllAsTouched();
      return;
    }

    const loginWithAuthenticatorRequest: loginWithAuthenticatorRequest = {
      userIdentifier: this.ui,
      code: this.codeControls.value.join('')
    }

    this.isLoadingResponse = true;
    this.loginService.loginWithAuthenticator(loginWithAuthenticatorRequest)
      .pipe(
        finalize(() => {
          this.isLoadingResponse = false;
        })
      ).subscribe({
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
              this.toatsService.error(translations.title, translations.message, 5000);
            });
          }
        }
      });
  }
}