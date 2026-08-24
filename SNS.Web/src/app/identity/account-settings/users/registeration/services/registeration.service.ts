import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environments/environment.development';
import { Result } from '../../../../../shared/contracts/result';
import { RegisterUserCommand } from '../contracts/register-user-command.dto';
import { VerifyUserCommand } from '../contracts/verify-user-command.dto';
import { ResendVerifyCodeCommand } from '../contracts/resend-verify-code-command.dto';
import { RegisterResponse } from '../contracts/register-response.dto';
import { AuthTokenDto } from '../../../../shared/contracts/auth-token.dto';

@Injectable({
    providedIn: 'root'
})
export class RegisterationService {
    private apiUrl = environment.apiUrl + 'Registeration';
    private http = inject(HttpClient);

    public register(request: RegisterUserCommand): Observable<Result<RegisterResponse>> {
        return this.http.post<Result<RegisterResponse>>(this.apiUrl, request);
    }

    public verifyUser(request: VerifyUserCommand): Observable<Result<AuthTokenDto>> {
        return this.http.post<Result<AuthTokenDto>>(`${this.apiUrl}/verify-user`, request);
    }

    public resendVerificationCode(request: ResendVerifyCodeCommand): Observable<Result<RegisterResponse>> {
        return this.http.post<Result<RegisterResponse>>(`${this.apiUrl}/resend-verify-code`, request);
    }
}
