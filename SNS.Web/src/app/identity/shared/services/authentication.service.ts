import { inject, Injectable, signal } from "@angular/core";
import { StorageKey, StorageService } from "../../../shared/services/storage.service";

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {
    private storageService = inject(StorageService);

    public isAuthenticated = signal<boolean>(false);
    public currentRole = signal<string>('');

    constructor() {
        this.refreshAuthenticationState();
    }

    public setAccessToken(token: string): void {
        this.storageService.set(StorageKey.AccessToken, token);
        this.refreshAuthenticationState();
    }

    public getAccessToken(): string | null {
        return this.storageService.get(StorageKey.AccessToken);
    }

    public removeToken(): void {
        this.storageService.remove(StorageKey.AccessToken);
        this.refreshAuthenticationState();
    }

    public getClaim(claimName: string): string | null {
        const token = this.getAccessToken();
        if (!token) return null;
        const claims = this.parseClaims(token);
        return claims[claimName] || null;
    }

    public getUserId(): string | null {
        return this.getClaim('sub');
    }

    public refreshAuthenticationState(): void {
        const token = this.getAccessToken();
        if (token) {
            this.isAuthenticated.set(true);
            const role = this.getClaim('role');
            this.currentRole.set(role || '');
        } else {
            this.isAuthenticated.set(false);
            this.currentRole.set('');
        }
    }

    public checkAuthentication(): void {
        this.refreshAuthenticationState();
    }

    private parseClaims(token: string): Record<string, any> {
        try {
            const base64Url = token.split('.')[1];
            if (!base64Url) return {};

            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            return JSON.parse(jsonPayload);
        } catch (error) {
            return {};
        }
    }
}