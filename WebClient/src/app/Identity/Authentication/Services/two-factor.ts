import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ValidateTwoFactorRequestDto } from '../DTOs/validate-two-factor';
import { Observable } from 'rxjs';
import { AuthTokenDto } from '../../Shared/DTOs/auth-token';
import { ResendTwoFactorCodeDto } from '../DTOs/resend-two-factor-code';

@Injectable({
  providedIn: 'root',
})
export class TwoFactorService {
  private readonly apiUrl = environment.apiUrl + "identity/two-factor/";

  constructor(private readonly http: HttpClient) { }

  public Validate(command: ValidateTwoFactorRequestDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}validate`, command);
  }

  public ResendCode(command: ResendTwoFactorCodeDto): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}resend`, command);
  }
}
