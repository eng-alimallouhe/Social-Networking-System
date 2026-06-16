import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthTokenDto } from '../../../Shared/DTOs/auth-token';
import { InitialEmailChagne } from '../components/initial-email-chagne/initial-email-chagne';
import { VerifyEmailChangeDto } from '../dtos/verify-email-change';

@Injectable({
  providedIn: 'root',
})
export class EmailChange {
  private readonly apiUrl = environment.apiUrl + "identity/email-change/";

  constructor(private readonly http: HttpClient) { }

  public InitiateChange(command: InitialEmailChagne): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}initiate-change`, command);
  }

  public ResendVerificationCode(): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}resend-verification-code`, {});
  }

  public VerifyChange(command: VerifyEmailChangeDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}verify-change`, command);
  }
}