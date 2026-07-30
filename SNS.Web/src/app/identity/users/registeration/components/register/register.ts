import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, forkJoin } from 'rxjs';
import { LucideLockKeyhole, LucideSquareUserRound, LucideMail, LucideUserPlus } from '@lucide/angular';
import { Result } from '../../../../../shared/contracts/result';
import { RegisterResponse } from '../../contracts/register-response.dto';
import { RegisterUserCommand } from '../../contracts/register-user-command.dto';
import { RegisterationService } from '../../services/registeration.service';
import { ToastService } from '../../../../notifications/services/toast.service';
import { AuthFlowService } from '../../../../shared/services/auth-flow.service';
import { LoadingAuthService } from '../../../../shared/layout/services/loading-auth.service';
import { passwordMatchValidator } from '../../../../../shared/validators/password-match.validator';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
        ReactiveFormsModule,
        LucideSquareUserRound,
        LucideMail,
        LucideLockKeyhole,
        LucideUserPlus
    ],
    templateUrl: './register.html',
    styleUrl: './register.css'
})
export class Register {
    private fb = inject(FormBuilder);
    private registerService = inject(RegisterationService);
    private toastService = inject(ToastService);
    private authFlowService = inject(AuthFlowService);
    private router = inject(Router);
    private loadingService = inject(LoadingAuthService);
    private translateService = inject(TranslateService);

    isLoading = this.loadingService.isLoading;

    registerForm: FormGroup = this.fb.group(
        {
            username: ['', [Validators.required, Validators.minLength(3)]],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(8)]],
            confirmPassword: ['', [Validators.required]]
        },
        { validators: passwordMatchValidator('password', 'confirmPassword') }
    );

    onSubmit(): void {
        if (this.registerForm.invalid) {
            this.registerForm.markAllAsTouched();
            return;
        }

        const formValue = this.registerForm.value;
        const request: RegisterUserCommand = {
            username: formValue.username,
            email: formValue.email,
            password: formValue.password,
            confirmPassword: formValue.confirmPassword
        };

        this.loadingService.show();

        this.registerService.register(request)
            .pipe(finalize(() => this.loadingService.hide()))
            .subscribe({
                next: (response: Result<RegisterResponse>) => {
                    const registerResponse = response.value;
                    this.authFlowService.setTfaRecipientAddress(request.email);
                    this.router.navigate(['/auth/verify-account'], {
                        queryParams: {
                            uid: registerResponse?.userId,
                            'challenge-token': registerResponse?.challengeToken
                        }
                    });
                },
                error: (err) => this.handleError(err)
            });
    }

    private handleError(err: any): void {
        const errorResult = err.error as Result<RegisterResponse>;
        if (errorResult?.statusCode) {
            const { category, code } = errorResult.statusCode;
            forkJoin({
                message: this.translateService.get(`Status_Codes.${category}.${code}`),
                title: this.translateService.get('Status_Codes.Shared.Error_Title')
            }).subscribe(translations => {
                this.toastService.error(translations.title, translations.message, 5000);
            });
        }
    }
}
