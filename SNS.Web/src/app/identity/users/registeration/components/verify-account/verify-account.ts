import { Component, ElementRef, OnInit, OnDestroy, QueryList, ViewChildren, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, forkJoin } from 'rxjs';
import { LucideMail, LucideCircleCheck } from '@lucide/angular';
import { Result } from '../../../../../shared/contracts/result';
import { RegisterResponse } from '../../contracts/register-response.dto';
import { VerifyUserCommand } from '../../contracts/verify-user-command.dto';
import { ResendVerifyCodeCommand } from '../../contracts/resend-verify-code-command.dto';
import { RegisterationService } from '../../services/registeration.service';
import { ToastService } from '../../../../notifications/services/toast.service';
import { AuthFlowService } from '../../../../shared/services/auth-flow.service';
import { LoadingAuthService } from '../../../../shared/layout/services/loading-auth.service';
import { GeneratorService } from '../../../../../shared/services/generator.service';

@Component({
    selector: 'app-verify-account',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        RouterLink,
        TranslatePipe,
        LucideMail,
        LucideCircleCheck
    ],
    templateUrl: './verify-account.html',
    styleUrl: './verify-account.css'
})
export class VerifyAccount implements OnInit, OnDestroy {
    private registerService = inject(RegisterationService);
    private loadingService = inject(LoadingAuthService);
    private toastService = inject(ToastService);
    private authFlowService = inject(AuthFlowService);
    private generatorService = inject(GeneratorService);
    private translateService = inject(TranslateService);
    private route = inject(ActivatedRoute);
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

    @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef<HTMLInputElement>>;

    constructor() {
        this.otpForm = this.fb.group({
            code: this.fb.array(
                Array(6).fill(null).map(() =>
                    this.fb.control('', [Validators.required, Validators.pattern('^[0-9]$')])
                )
            )
        });
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.userId = params['uid'] || '';
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

    get codeControls(): FormArray {
        return this.otpForm.get('code') as FormArray;
    }

    // ── OTP input handlers (identical to verify-otp.ts) ──────────────
    onFocus(event: FocusEvent): void {
        (event.target as HTMLInputElement).select();
    }

    onInput(event: Event, index: number): void {
        const input = event.target as HTMLInputElement;
        const val = input.value.replace(/[^0-9]/g, '');
        input.value = val.length > 1 ? val.charAt(val.length - 1) : val;
        this.codeControls.at(index).setValue(input.value);
        if (input.value && index < 5) {
            this.codeInputs.toArray()[index + 1].nativeElement.focus();
        }
    }

    onKeyDown(event: KeyboardEvent, index: number): void {
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

    onPaste(event: ClipboardEvent): void {
        event.preventDefault();
        const pasted = event.clipboardData?.getData('text/plain');
        if (!pasted) return;
        const nums = pasted.replace(/\D/g, '').substring(0, 6);
        for (let i = 0; i < nums.length; i++) {
            this.codeControls.at(i).setValue(nums[i]);
        }
        if (nums.length > 0) {
            this.codeInputs.toArray()[Math.min(nums.length, 5)].nativeElement.focus();
        }
    }

    // ── Submit ────────────────────────────────────────────────────────
    onSubmit(): void {
        if (this.otpForm.invalid) {
            this.otpForm.markAllAsTouched();
            return;
        }

        const request: VerifyUserCommand = {
            userId: this.userId,
            challengeToken: this.challengeToken,
            code: this.codeControls.value.join('')
        };

        this.loadingService.show();
        this.registerService.verifyUser(request)
            .pipe(finalize(() => this.loadingService.hide()))
            .subscribe({
                next: () => {
                    forkJoin({
                        message: this.translateService.get('Identity.Users.Registration.Verify_Account.Success_Body'),
                        title: this.translateService.get('Identity.Users.Registration.Verify_Account.Success_Title')
                    }).subscribe(translations => {
                        this.toastService.success(translations.title, translations.message, 5000);
                    });
                    this.authFlowService.clear();
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
