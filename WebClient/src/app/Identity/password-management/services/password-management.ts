import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ChangePasswordDto } from '../dtos/change-password';
import { ForgotPasswordDto } from '../dtos/forgot-password';
import { ResetPasswordDto } from '../dtos/reset-password';
import { VerifyResetCodeDto } from '../dtos/verify-reset-code';
import { Observable } from 'rxjs';
import { AuthTokenDto } from '../../Shared/DTOs/auth-token';

@Injectable({
  providedIn: 'root',
})
export class PasswordManagement {
  private readonly apiUrl = environment.apiUrl + "identity/password/";

  constructor(private readonly http: HttpClient) { }

  public ChangePassword(command: ChangePasswordDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}change`, command);
  }

  public ForgotPassword(command: ForgotPasswordDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}forgot`, command);
  }

  public ResetPassword(command: ResetPasswordDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}reset`, command);
  }

  public VerifyReset(command: VerifyResetCodeDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}verify-reset`, command);
  }
}
