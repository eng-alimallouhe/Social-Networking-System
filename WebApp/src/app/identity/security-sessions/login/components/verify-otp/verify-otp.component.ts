import { Component, ElementRef, OnInit, QueryList, ViewChildren, inject, OnDestroy, signal, Signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { finalize, forkJoin, interval, Subscription } from 'rxjs';

import { LinearLoaderComponent } from '../../../../../shared/components/loaders/linear-loader/linear-loader.component';
import { AuthFlowService } from '../../../../shared/services/auth-flow.service';
import { LoginService } from '../../services/login.service';
import { TokenService } from '../../../../shared/services/token.service';
import { ToastService } from '../../../../../shared/services/toast.service';
import { RequestInformationService } from '../../../../shared/services/request-information.service';
import { Result } from '../../../../../shared/contracts/result';
import { LoginResponse } from '../../contracts/login-response.dto';
import { ValidateTwoFactorRequest } from '../../contracts/validate-two-factor-reqest.dto';
import { ResendTwoFactorCodeRequest } from '../../contracts/resend-two-factor-code-request.dto';
import { GeneratorService } from '../../../../../shared/services/generator.service';


@Component({
  selector: 'app-verify-otp',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslateModule, LinearLoaderComponent],
  templateUrl: './verify-otp.component.html',
  styleUrl: './verify-otp.component.css'
})
export class VerifyOtpComponent implements OnInit, OnDestroy {
  private loginService = inject(LoginService);
  private tokenService = inject(TokenService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private toastService = inject(ToastService);
  private translateService = inject(TranslateService);
  private requestInfoService = inject(RequestInformationService);
  private authFlowService = inject(AuthFlowService);
  private generatorService = inject(GeneratorService);

  otpForm: FormGroup;
  isLoadingResponse = false;
  isResending = false;

  // المتغيرات المستلمة
  userId: string = '';
  challengeToken: string = '';
  recipientAddress: string = '';

  // العداد الزمني لإعادة الإرسال
  resendCooldown = signal(60);
  canResend = computed(() => this.resendCooldown() <= 0);
  private timerSubscription?: Subscription;

  @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef<HTMLInputElement>>;

  constructor() {
    this.otpForm = this.fb.group({
      code: this.fb.array(
        Array(6).fill(null).map(() => this.fb.control('', [Validators.required, Validators.pattern('^[0-9]$')]))
      )
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.userId = params['uid'] || '';
      this.challengeToken = params['challenge-token'] || '';
    });

    this.recipientAddress = this.generatorService.generateEmailMask(this.authFlowService.getTfaRecipientAddress() || '@gmail.com');

    if (!this.userId) {
      this.router.navigate(['/auth/login']);
      return;
    }

    this.startResendTimer();
  }

  ngOnDestroy() {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }
  }

  get codeControls() { return this.otpForm.get('code') as FormArray; }

  // [نفس دوال التحكم بالخانات من مكون Authenticator تظل هنا: onFocus, onInput, onKeyDown, onPaste]
  onFocus(event: FocusEvent) { const input = event.target as HTMLInputElement; input.select(); }
  onInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    const val = input.value.replace(/[^0-9]/g, '');
    input.value = val.length > 1 ? val.charAt(val.length - 1) : val;
    this.codeControls.at(index).setValue(input.value);
    if (input.value && index < 5) this.codeInputs.toArray()[index + 1].nativeElement.focus();
  }
  onKeyDown(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;
    if (event.key === 'Backspace') {
      if (!input.value && index > 0) {
        const prev = this.codeInputs.toArray()[index - 1];
        prev.nativeElement.value = '';
        this.codeControls.at(index - 1).setValue('');
        prev.nativeElement.focus();
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
    const pasted = event.clipboardData?.getData('text/plain');
    if (!pasted) return;
    const nums = pasted.replace(/\D/g, '').substring(0, 6);
    for (let i = 0; i < nums.length; i++) this.codeControls.at(i).setValue(nums[i]);
    if (nums.length > 0) this.codeInputs.toArray()[Math.min(nums.length, 5)].nativeElement.focus();
  }

  // إرسال الرمز للتحقق
  onSubmit() {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    const request: ValidateTwoFactorRequest = {
      userId: this.userId,
      token: this.challengeToken,
      code: this.codeControls.value.join('')
    };

    this.isLoadingResponse = true;
    this.loginService.validateTfaCode(request)
      .pipe(finalize(() => this.isLoadingResponse = false))
      .subscribe({
        next: (response) => {
          this.tokenService.setToken(response.value?.accessToken!, response.value?.refreshToken!);
          this.requestInfoService.setDeviceId(response.value?.deviceId!);
          this.router.navigate(['/']);
        },
        error: (err) => this.handleError(err)
      });
  }

  // إعادة إرسال الرمز
  resendCode() {
    if (!this.canResend) return;

    const request: ResendTwoFactorCodeRequest = { userId: this.userId };

    this.isResending = true;
    this.loginService.resendTfaCode(request)
      .pipe(finalize(() => this.isResending = false))
      .subscribe({
        next: () => {
          this.toastService.success("تم إرسال الرمز الجديد بنجاح", "نجاح");
          this.startResendTimer(); // إعادة تشغيل العداد
        },
        error: (err) => this.handleError(err)
      });
  }

  // عداد تنازلي لإعادة الإرسال
  private startResendTimer() {
    this.resendCooldown.set(60);

    const intervalId = setInterval(() => {
      this.resendCooldown.update(val => val - 1);

      if (this.resendCooldown() <= 0) {
        clearInterval(intervalId);
      }
    }, 1000);
  }

  private handleError(err: any) {
    var errorResult = err.error as Result<any>;
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
}