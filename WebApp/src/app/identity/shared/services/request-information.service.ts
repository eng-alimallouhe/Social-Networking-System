import { inject, Injectable } from "@angular/core";
import { StorageKey, StorageService } from "../../../shared/services/storage.service";
import FingerprintJS from '@fingerprintjs/fingerprintjs';



@Injectable({
    providedIn: 'root'
})
export class RequestInformationService {
    private storageService = inject(StorageService);

    setDeviceId(deviceId: string) {
        this.storageService.set(StorageKey.DeviceId, deviceId);
    }

    setDeviceToken() {
        const token = generateUUID();
        this.storageService.set(StorageKey.DeviceToken, token);
    }

    setFingerprintHash() {
        const fpPromise = FingerprintJS.load();
        fpPromise.then(fp => fp.get()).then(result => {
            const visitorId = result.visitorId;
            this.storageService.set(StorageKey.FingerprintHash, visitorId);
        });
    }

    getDeviceId(): string {
        return this.storageService.get(StorageKey.DeviceId) ?? '';
    }

    getDeviceToken(): string {
        if (!this.storageService.get(StorageKey.DeviceToken)) {
            this.setDeviceToken();
        }
        return this.storageService.get(StorageKey.DeviceToken) ?? '';
    }

    getFingerprintHash(): string {
        if (!this.storageService.get(StorageKey.FingerprintHash)) {
            this.setFingerprintHash();
        }
        return this.storageService.get(StorageKey.FingerprintHash) ?? '';
    }
}

export function generateUUID(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}