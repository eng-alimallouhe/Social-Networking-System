import { Injectable, signal } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class SecuritySettingsStateService {

    private readonly _settingsChanged = signal(0);

    readonly settingsChanged = this._settingsChanged.asReadonly();

    notifySettingsChanged(): void {
        this._settingsChanged.update(value => value + 1);
    }

    resetSettingsChanged(): void {
        this._settingsChanged.set(0);
    }
}