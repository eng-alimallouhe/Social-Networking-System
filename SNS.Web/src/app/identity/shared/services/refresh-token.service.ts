import { Injectable, inject } from "@angular/core";
import { tap } from "rxjs";
import { TokenService } from "./token.service";
import { SessionManagementService } from "../../account-settings/security-sessions/session-management/services/session-management.service";

@Injectable({
    providedIn: 'root'
})
export class RefreshTokenService {

    private sessionService = inject(SessionManagementService);
    private tokenService = inject(TokenService);

    refresh() {
        return this.sessionService.refreshTokens().pipe(
            tap(result => {
                this.tokenService.setAccessToken(
                    result.value?.token!
                );
            })
        );
    }
}