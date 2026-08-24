import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../../environments/environment.development';
import { Result } from '../../../../../shared/contracts/result';
import { AuthenticatorSetupDto } from '../contracts/authenticator-setup.dto';
import { ChangeMfaProviderCommand } from '../contracts/change-mfa-provider-request.dto';
import { EnableMFACommand } from '../contracts/enable-mfa-request.dto';
import { CompleteAuthenticatorRegistrationCommand } from '../contracts/complete-authenticator-registration-request.dto';
import { InitialRecoveryEmailChangeCommand } from '../contracts/initial-recovery-email-change-request.dto';
import { VerifyRecoveryEmailChangeCommand } from '../contracts/verify-recovery-email-change-request.dto';
import { ChangeDefaultCommunicationMethodCommand } from '../contracts/change-default-communication-method-request.dto';
import { ResendRecoveryEmailChangeVerificationCodeCommand } from '../contracts/resend-recovery-email-change-verification-code-request.dto';
import { IdentifierChangeResponseDto } from '../../email-change/contracts/identifier-change-response.dto';
import { PasskeyDto } from '../contracts/passkey.dto';
import { RemovePasskeyCommand } from '../contracts/remove-passkey-request.dto';
import { InitiatePasskeyRegistrationCommand, CredentialCreateOptionsDto } from '../contracts/initiate-passkey-registration.dto';
import { CompletePasskeyRegistrationCommand } from '../contracts/complete-passkey-registration.dto';

@Injectable({
  providedIn: 'root',
})
export class MfaManagementService {
  private apiUrl = environment.apiUrl + 'identity/security-settings/MfaManagement';
  private http = inject(HttpClient);

  public changeMfaProvider(request: ChangeMfaProviderCommand): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/change-mfa-provider`, request);
  }

  public initiateAuthenticatorRegistration(): Observable<Result<AuthenticatorSetupDto>> {
    return this.http.post<Result<AuthenticatorSetupDto>>(`${this.apiUrl}/initiate-authenticator-registration`, {});
  }

  public completeAuthenticatorRegistration(request: CompleteAuthenticatorRegistrationCommand): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/complete-authenticator-registration`, request);
  }

  public initialRecoveryEmailChange(request: InitialRecoveryEmailChangeCommand): Observable<Result<IdentifierChangeResponseDto>> {
    return this.http.post<Result<IdentifierChangeResponseDto>>(`${this.apiUrl}/initial-recovery-email-change`, request);
  }

  public resendRecoveryEmailChangeCode(request: ResendRecoveryEmailChangeVerificationCodeCommand): Observable<Result<IdentifierChangeResponseDto>> {
    return this.http.post<Result<IdentifierChangeResponseDto>>(`${this.apiUrl}/resend-recovery-email-change-verification-code`, request);
  }

  public verifyRecoveryEmailChange(request: VerifyRecoveryEmailChangeCommand): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/-verify-recovery-email-change`, request);
  }

  public changeDefaultCommunicationMethod(request: ChangeDefaultCommunicationMethodCommand): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/change-default-communication-method`, request);
  }

  public disableMfa(): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/disable-mfa`, {});
  }

  public enableMfa(request: EnableMFACommand): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/enable-mfa`, request);
  }

  public getUserPasskeys(): Observable<Result<PasskeyDto[]>> {
    return this.http.get<Result<PasskeyDto[]>>(`${this.apiUrl}/user-passkeys`);
  }

  public removePasskey(request: RemovePasskeyCommand): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/remove-passkey`, request);
  }

  public initiatePasskeyRegistration(request: InitiatePasskeyRegistrationCommand): Observable<Result<CredentialCreateOptionsDto>> {
    return this.http.post<Result<CredentialCreateOptionsDto>>(`${this.apiUrl}/initiate-passkey-registration`, request);
  }

  public completePasskeyRegistration(request: CompletePasskeyRegistrationCommand): Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}/complete-passkey-registration`, request);
  }
}
