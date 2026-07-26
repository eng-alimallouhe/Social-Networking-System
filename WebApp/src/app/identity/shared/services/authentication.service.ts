import { Injectable, inject, signal } from "@angular/core";
import { TokenService } from "../services/token.service";

@Injectable({
    providedIn: 'root',
})
export class AuthenticationService {
    private tokenService = inject(TokenService);
    public isAuthenticated = signal<boolean>(false);

    constructor() {
        this.checkIfUserIsAuthenticated();
    }

    public checkIfUserIsAuthenticated(): boolean {
        const token = this.tokenService.getAccessToken();
        if (token) {
            this.isAuthenticated.set(true);
        }
        return this.isAuthenticated();
    }
}