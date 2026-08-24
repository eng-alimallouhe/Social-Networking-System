import { Component, ElementRef, OnInit, QueryList, ViewChildren, inject, OnDestroy, signal, Signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize, Subscription } from 'rxjs';
import { AuthFlowService } from '../../../../../shared/services/auth-flow.service';
import { LoginService } from '../../services/login.service';
import { TokenService } from '../../../../../shared/services/token.service';
import { ToastService } from '../../../../../notifications/services/toast.service';
import { RequestInformationService } from '../../../../../shared/services/request-information.service';
import { ValidateTwoFactorRequest } from '../../contracts/validate-two-factor-reqest.dto';
import { ResendTwoFactorCodeRequest } from '../../contracts/resend-two-factor-code-request.dto';
import { GeneratorService } from '../../../../../../shared/services/generator.service';
import { LucideCircleCheck, LucideMail } from "@lucide/angular";
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { AppCodeInput } from '../../../../../../shared/design-system/components/app-code-input/app-code-input';
import { Result } from '../../../../../../shared/contracts/result';
import { OtpChallengeDto } from '../../contracts/otp-challenge.dto';


@Component({
  selector: 'app-verify-otp',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslatePipe, LucideCircleCheck, LucideMail, AppCodeInput],
  templateUrl: './verify-otp.html',
  styleUrl: './verify-otp.css'
})
export class VerifyOtp implements OnInit, OnDestroy {
  private loginService = inject(LoginService);
  private tokenService = inject(TokenService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private toastService = inject(ToastService);
  private requestInfoService = inject(RequestInformationService);
  private authFlowService = inject(AuthFlowService);
  private generatorService = inject(GeneratorService);
  private loadingService = inject(GlobalLoaderService);

  isLoading = this.loadingService.isLoading;

  otpForm: FormGroup;

  userId: string = '';
  challengeToken: string = '';
  recipientAddress: string = '';

  resendCooldown = signal(60);
  canResend = computed(() => this.resendCooldown() <= 0);
  private timerSubscription?: Subscription;

  @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef<HTMLInputElement>>;

  constructor() {
    this.otpForm = this.fb.group({
      code: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
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

  onSubmit() {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    const request: ValidateTwoFactorRequest = {
      userId: this.userId,
      token: this.challengeToken,
      code: this.otpForm.value.code
    };

    this.loadingService.show();
    this.loginService.validateTfaCode(request)
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (response) => {
          this.tokenService.setToken(response.value?.accessToken!);
          this.requestInfoService.setDeviceId(response.value?.deviceId!);
          this.router.navigate(['/']);
        }
      });
  }

  resendCode() {
    if (!this.canResend) return;

    const request: ResendTwoFactorCodeRequest = { userId: this.userId };

    this.loadingService.show()
    this.loginService.resendTfaCode(request)
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe({
        next: (response: Result<OtpChallengeDto>) => {
          if (response.value !== null) {
            this.challengeToken = response.value.challengeToken!;
            this.userId = response.value.userId;
          }

          const titleKey = `Status_Codes.Shared.Success_Title`;
          const messageKey = `Status_Codes.${response.statusCode.category}.${response.statusCode.code}`;
          this.toastService.success(titleKey, messageKey);

          this.otpForm.reset();
          this.startResendTimer();
        }
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

}