import { inject, Injectable } from "@angular/core";
import { StorageKey, StorageService } from "../../../shared/services/storage.service";

@Injectable({
    providedIn: 'root'
})
export class TokenService {
    private storageService = inject(StorageService);

    setToken(accessToken: string, refreshToken: string): void {
        this.storageService.set(StorageKey.AccessToken, accessToken);
        this.storageService.set(StorageKey.RefreshToken, refreshToken);
    }

    getAccessToken(): string | null {
        return this.storageService.get(StorageKey.AccessToken);
    }

    getRefreshToken(): string | null {
        return this.storageService.get(StorageKey.RefreshToken);
    }

    removeToken(): void {
        this.storageService.remove(StorageKey.AccessToken);
        this.storageService.remove(StorageKey.RefreshToken);
    }

    setAccessToken(token: string): void {
        this.storageService.set(StorageKey.AccessToken, token);
    }

    getClaim(claimName: string): string | null {
        const token = this.getAccessToken();
        if (!token) return null;
        const claims = this.parseClaims(token);
        return claims[claimName] || null;
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
            console.error('Failed to parse JWT claims', error);
            return {};
        }
    }
}