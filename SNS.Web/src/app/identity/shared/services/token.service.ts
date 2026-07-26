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

    isAuthenticated(): boolean {
        return !!this.storageService.get(StorageKey.AccessToken);
    }
}