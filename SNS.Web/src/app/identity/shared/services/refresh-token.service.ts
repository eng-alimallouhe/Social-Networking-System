import { Injectable, inject } from "@angular/core";
import { tap } from "rxjs";
import { LoginService } from "../../security-sesstions/login/services/login.service";
import { TokenService } from "./token.service";

@Injectable({
    providedIn: 'root'
})
export class RefreshTokenService {

    private loginService = inject(LoginService);
    private tokenService = inject(TokenService);

    refresh() {
        return this.loginService.refreshToken().pipe(

            tap(result => {
                this.tokenService.setAccessToken(
                    result.value?.token!
                );
            })
        );
    }
}