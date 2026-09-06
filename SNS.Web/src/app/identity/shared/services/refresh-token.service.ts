import { Injectable, inject } from "@angular/core";
import { tap } from "rxjs";
import { AuthenticationService } from "./authentication.service";
import { SessionManagementService } from "../../account-settings/security-sessions/session-management/services/session-management.service";

@Injectable({
    providedIn: 'root'
})
export class RefreshTokenService {

    private sessionService = inject(SessionManagementService);
    private authenticationService = inject(AuthenticationService);

    refresh() {
        return this.sessionService.refreshTokens().pipe(
            tap(result => {
                this.authenticationService.setAccessToken(
                    result.value?.token!
                );
            })
        );
    }
}