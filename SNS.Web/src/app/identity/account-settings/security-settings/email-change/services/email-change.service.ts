import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { InitialEmailChangeRequest } from '../contracts/initial-email-change-request.dto';
import { Observable } from 'rxjs';
import { Result } from '../../../../../shared/contracts/result';
import { IdentifierChangeResponseDto } from '../contracts/identifier-change-response.dto';
import { AuthTokenDto } from '../../../../shared/contracts/auth-token.dto';
import { VerifyEmailChangeRequest } from '../contracts/verify-email-change-request.dt';

@Injectable({
  providedIn: 'root',
})
export class EmailChangeService {
  private apiUrl = environment.apiUrl + 'identity/security-settings/EmailChange';
  private http = inject(HttpClient);

  public initialEmailChange(request: InitialEmailChangeRequest): Observable<Result<IdentifierChangeResponseDto>> {
    return this.http.put<Result<IdentifierChangeResponseDto>>(`${this.apiUrl}/initiate-email-change`, request);
  }

  public resendEmailChangeCode(): Observable<Result<IdentifierChangeResponseDto>> {
    return this.http.put<Result<IdentifierChangeResponseDto>>(`${this.apiUrl}/resend-email-change-code`, {});
  }

  public confirmEmailChange(request: VerifyEmailChangeRequest): Observable<Result<AuthTokenDto>> {
    return this.http.put<Result<AuthTokenDto>>(`${this.apiUrl}/verify-email-change`, request);
  }
}