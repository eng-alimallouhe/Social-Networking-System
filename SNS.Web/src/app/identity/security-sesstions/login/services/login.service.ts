import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { LoginWithPasswordRequest } from '../contracts/login-with-password-request.dto';
import { Observable } from 'rxjs';
import { LoginResponse } from '../contracts/login-response.dto';
import { loginWithAuthenticatorRequest } from '../contracts/login-with-authenticator-request.dto';
import { ValidateTwoFactorRequest } from '../contracts/validate-two-factor-reqest.dto';
import { ResendTwoFactorCodeRequest } from '../contracts/resend-two-factor-code-request.dto';
import { Result } from '../../../../shared/contracts/result';


@Injectable({
  providedIn: 'root',
})
export class LoginService {
  private apiUrl = environment.apiUrl + 'identity/security-sessions/login';
  private http = inject(HttpClient);

  public loginWithPassword(request: LoginWithPasswordRequest): Observable<Result<LoginResponse>> {
    return this.http.post<Result<LoginResponse>>(`${this.apiUrl}/with-password`, request);
  }

  public loginWithAuthenticator(request: loginWithAuthenticatorRequest): Observable<Result<LoginResponse>> {
    return this.http.post<Result<LoginResponse>>(`${this.apiUrl}/with-authenticator-app`, request);
  }

  public initiateLogin(identifier: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/initiate-passkey-login`, { identifier });
  }

  public completeLogin(payload: any): Observable<Result<LoginResponse>> {
    return this.http.post<Result<LoginResponse>>(`${this.apiUrl}/complete-passkey-login`, payload);
  }


  public validateTfaCode(request: ValidateTwoFactorRequest): Observable<Result<LoginResponse>> {
    return this.http.post<Result<LoginResponse>>(`${this.apiUrl}/validate-tfa-code`, request);
  }

  public resendTfaCode(request: ResendTwoFactorCodeRequest): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/resend-tfa-code`, request);
  }
}