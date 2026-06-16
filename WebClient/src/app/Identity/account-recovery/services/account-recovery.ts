import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthTokenDto } from '../../Shared/DTOs/auth-token';

@Injectable({
  providedIn: 'root',
})
export class AccountRecovery {
  private readonly apiUrl = environment.apiUrl + "identity/recovery/";

  constructor(private readonly http: HttpClient) { }

  public RecoverAccountBySecurityCode(code: string): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}recover-by-security-code`, code);
  }
}
