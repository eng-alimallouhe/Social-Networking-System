import { Injectable, signal } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class ThemeService {
    private _currentTheme = signal(ThemeType.Light);

    public currentTheme = this._currentTheme.asReadonly();

    public changeTheme(theme: ThemeType): void {
        this._currentTheme.set(theme);
    }
}


export enum ThemeType {
    Light = 'Light',
    Dark = 'Dark'
}