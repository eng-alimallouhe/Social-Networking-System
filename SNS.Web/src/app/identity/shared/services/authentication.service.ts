import { inject, Injectable, signal } from "@angular/core";
import { TokenService } from "./token.service";

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {
    private tokenService = inject(TokenService);

    public isAuthenticated = signal<boolean>(false);
    public currentRole = signal<string>('');

    constructor() {
        this.checkIfUserIsAuthenticated();
        this.extractRole();
    }

    private checkIfUserIsAuthenticated() {
        const token = this.tokenService.getAccessToken();
        if (token) {
            this.isAuthenticated.set(true);
        }
        else {
            this.isAuthenticated.set(false);
        }
    }

    private extractRole(): void {
        const token = this.tokenService.getAccessToken();
        if (token) {
            const role = this.tokenService.getClaim('role');
            this.currentRole.set(role || '');
        }
    }
}