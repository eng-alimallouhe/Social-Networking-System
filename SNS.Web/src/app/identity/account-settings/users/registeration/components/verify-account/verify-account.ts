import { Component, ElementRef, OnInit, OnDestroy, QueryList, ViewChildren, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, forkJoin } from 'rxjs';
import { LucideMail, LucideCircleCheck } from '@lucide/angular';
import { Result } from '../../../../../../shared/contracts/result';
import { VerifyUserCommand } from '../../contracts/verify-user-command.dto';
import { ResendVerifyCodeCommand } from '../../contracts/resend-verify-code-command.dto';
import { RegisterationService } from '../../services/registeration.service';
import { ToastService } from '../../../../../notifications/services/toast.service';
import { AuthFlowService } from '../../../../../shared/services/auth-flow.service';
import { GeneratorService } from '../../../../../../shared/services/generator.service';
import { AppCodeInput } from '../../../../../../shared/design-system/components/app-code-input/app-code-input';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { TokenService } from '../../../../../shared/services/token.service';
import { AuthTokenDto } from '../../../../../shared/contracts/auth-token.dto';

@Component({
    selector: 'app-verify-account',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink,
        TranslatePipe,
        LucideMail,
        LucideCircleCheck,
        AppCodeInput
    ],
    templateUrl: './verify-account.html',
    styleUrl: './verify-account.css'
})
export class VerifyAccount implements OnInit, OnDestroy {
    private registerService = inject(RegisterationService);
    private loadingService = inject(GlobalLoaderService);
    private toastService = inject(ToastService);
    private authFlowService = inject(AuthFlowService);
    private generatorService = inject(GeneratorService);
    private translateService = inject(TranslateService);
    private route = inject(ActivatedRoute);
    private tokenService = inject(TokenService);
    private router = inject(Router);
    private fb = inject(FormBuilder);

    isLoading = this.loadingService.isLoading;

    // Route params
    userId: string = '';
    challengeToken: string = '';
    recipientAddress: string = '';

    // Resend countdown — same logic as verify-otp.ts
    resendCooldown = signal(60);
    canResend = computed(() => this.resendCooldown() <= 0);
    private timerIntervalId?: ReturnType<typeof setInterval>;

    otpForm: FormGroup;

    constructor() {
        this.otpForm = this.fb.group({
            code: ['', [Validators.required, Validators.pattern('^[0-9]{6}$')]]
        });
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.userId = params['ui'] || '';
            this.challengeToken = params['challenge-token'] || '';
        });

        this.recipientAddress = this.generatorService.generateEmailMask(
            this.authFlowService.getTfaRecipientAddress() || '@'
        );

        if (!this.userId) {
            this.router.navigate(['/auth/register']);
            return;
        }

        this.startResendTimer();
    }

    ngOnDestroy(): void {
        if (this.timerIntervalId) {
            clearInterval(this.timerIntervalId);
        }
    }

    // ── OTP input handlers replaced by AppCodeInput ──────────────

    // ── Submit ────────────────────────────────────────────────────────
    onSubmit(): void {
        if (this.otpForm.invalid) {
            this.otpForm.markAllAsTouched();
            return;
        }

        const request: VerifyUserCommand = {
            userId: this.userId,
            token: this.challengeToken,
            code: this.otpForm.value.code
        };

        this.loadingService.show();
        this.registerService.verifyUser(request)
            .pipe(finalize(() => this.loadingService.hide()))
            .subscribe({
                next: (response: Result<AuthTokenDto>) => {
                    forkJoin({
                        message: this.translateService.get('Identity.Users.Registration.Verify_Account.Success_Body'),
                        title: this.translateService.get('Identity.Users.Registration.Verify_Account.Success_Title')
                    }).subscribe(translations => {
                        this.toastService.success(translations.title, translations.message);
                    });
                    this.authFlowService.clear();

                    if (response.value !== null) {
                        this.tokenService.setAccessToken(response.value.token);
                    }

                    this.router.navigate(['/onboarding/create-profile']);
                }
            });
    }

    // ── Resend ────────────────────────────────────────────────────────
    resendCode(): void {
        if (!this.canResend() || this.isLoading()) return;

        const request: ResendVerifyCodeCommand = { userId: this.userId };

        this.loadingService.show();
        this.registerService.resendVerificationCode(request)
            .pipe(finalize(() => this.loadingService.hide()))
            .subscribe({
                next: () => {
                    forkJoin({
                        message: this.translateService.get('Identity.Users.Registration.Verify_Account.Resend_Success_Body'),
                        title: this.translateService.get('Identity.Users.Registration.Verify_Account.Resend_Success_Title')
                    }).subscribe(translations => {
                        this.toastService.success(translations.title, translations.message, 4000);
                    });
                    this.startResendTimer();
                }
            });
    }

    // ── Timer — identical to verify-otp.ts ───────────────────────────
    private startResendTimer(): void {
        if (this.timerIntervalId) {
            clearInterval(this.timerIntervalId);
        }
        this.resendCooldown.set(60);
        this.timerIntervalId = setInterval(() => {
            this.resendCooldown.update(val => val - 1);
            if (this.resendCooldown() <= 0) {
                clearInterval(this.timerIntervalId);
            }
        }, 1000);
    }

}
