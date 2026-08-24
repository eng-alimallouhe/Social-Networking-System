import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { LucideUserPlus } from '@lucide/angular';
import { Result } from '../../../../../../shared/contracts/result';
import { RegisterResponse } from '../../contracts/register-response.dto';
import { RegisterUserCommand } from '../../contracts/register-user-command.dto';
import { RegisterationService } from '../../services/registeration.service';
import { AuthFlowService } from '../../../../../shared/services/auth-flow.service';
import { passwordMatchValidator } from '../../../../../shared/validators/password-match.validator';
import { AppInput } from '../../../../../../shared/design-system/components/app-input/app-input';
import { GlobalLoaderService } from '../../../../../../shared/Loading/services/global-loader.service';
import { HttpErrorResponse } from '@angular/common/http';
import { isStatusCode } from '../../../../../../shared/contracts/status-code';
import { UserStatusCodes } from '../../../../../../shared/status-codes/user-status-codes';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        TranslatePipe,
        ReactiveFormsModule,
        LucideUserPlus,
        AppInput
    ],
    templateUrl: './register.html',
    styleUrl: './register.css'
})
export class Register {
    private fb = inject(FormBuilder);
    private registerService = inject(RegisterationService);
    private authFlowService = inject(AuthFlowService);
    private router = inject(Router);
    private loadingService = inject(GlobalLoaderService);

    isLoading = this.loadingService.isLoading;

    registerForm: FormGroup = this.fb.group(
        {
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
                    this.router.navigate(['/auth/register/verify-account'], {
                        queryParams: {
                            'ui': registerResponse?.userId,
                            'challenge-token': registerResponse?.token
                        }
                    });
                },
                error: (error: HttpErrorResponse) => {
                    if (isResult(error.error)) {
                        const result = error.error as Result<RegisterResponse>;
                        if (isStatusCode(result.statusCode, UserStatusCodes.NotVerified)) {
                            this.router.navigate(['/auth/register/verify-account'], {
                                queryParams: {
                                    'ui': result.value?.userId,
                                    'challenge-token': result.value?.token
                                }
                            });
                        }
                        else if (isStatusCode(result.statusCode, UserStatusCodes.ProfileNotCompleted)) {
                            setTimeout(() => {
                                this.router.navigate(['/auth/login/password']);
                            }, 5000);
                        }
                    }
                }
            });
    }
}

function isResult(value: unknown): value is Result {
    return !!value
        && typeof value === 'object'
        && 'statusCode' in value
        && 'isSuccess' in value;
}