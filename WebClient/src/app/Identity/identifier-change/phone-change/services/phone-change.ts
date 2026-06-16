import { Injectable } from '@angular/core';
import { VerifyPhoneChangeDto } from '../dtos/verify-phone-change';
import { InitialPhoneNumberChagne } from '../components/initial-phone-number-chagne/initial-phone-number-chagne';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { AuthTokenDto } from '../../../Shared/DTOs/auth-token';

@Injectable({
  providedIn: 'root',
})
export class PhoneChange {
  private readonly apiUrl = environment.apiUrl + "identity/phone-change/";

  constructor(private readonly http: HttpClient) { }

  public InitiateChange(command: InitialPhoneNumberChagne): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}initiate-change`, command);
  }

  public ResendVerificationCode(): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}resend-verification-code`, {});
  }

  public VerifyChange(command: VerifyPhoneChangeDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}verify-change`, command);
  }
}
