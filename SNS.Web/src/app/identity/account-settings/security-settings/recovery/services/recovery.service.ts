import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Result } from "../../../../../shared/contracts/result";
import { UserRecoveryCodesDto } from "../contracts/user-recovery-codes.dto";
import { IDENTITY_API_ROUTES } from "../../../../../shared/constants/api-routes/identity-api.routes";
import { AuthTokensDto } from "../../../../shared/contracts/auth-tokens.dto";
import { RecoverAccountBySecurityCodeCommand } from "../contracts/recover-account-by-security-code.command";

@Injectable({
    providedIn: 'root'
})
export class RecoveryService {
    private apiUrl = `${environment.apiUrl}${IDENTITY_API_ROUTES.Recovery}`;
    private http = inject(HttpClient);

    getUserRecoveryCodesHistory(): Observable<Result<UserRecoveryCodesDto>> {
        return this.http.get<Result<UserRecoveryCodesDto>>(`${this.apiUrl}`);
    }

    generateRecoveryCodes(): Observable<Result<string[]>> {
        return this.http.get<Result<string[]>>(`${this.apiUrl}/generate-recovery-codes`);
    }

    revokeRecoveryCodes(): Observable<Result<void>> {
        return this.http.post<Result<void>>(`${this.apiUrl}/revoke-recovery-codes`, {});
    }

    recoverAccountByRecoveryCode(command: RecoverAccountBySecurityCodeCommand): Observable<Result<AuthTokensDto>> {
        return this.http.post<Result<AuthTokensDto>>(`${this.apiUrl}/recover-account-by-recovery-code`, command);
    }
}