import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { RegisterUser } from '../DTOs/register-user';
import { ActivateAccount } from '../DTOs/activate-account';
import { ResendActivationCode } from '../DTOs/resend-activation-code';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthTokenDto } from '../../Shared/DTOs/auth-token';

@Injectable({
  providedIn: 'root',
})
export class Registration {
  private readonly apiUrl = environment.apiUrl + "identity/registration/";

  constructor(private readonly http: HttpClient) { }

  public Register(command: RegisterUser): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}register`, command);
  }

  public Activate(command: ActivateAccount): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}activate`, command);
  }

  public ResendActivation(command: ResendActivationCode): Observable<AuthTokenDto> {
    return this.http.post<AuthTokenDto>(`${this.apiUrl}resend-activation`, command);
  }

}
