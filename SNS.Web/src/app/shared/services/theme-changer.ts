import { inject, Injectable, signal, effect } from '@angular/core';
import { StorageKey, StorageService } from './storage.service';

@Injectable({
  providedIn: 'root',
})
export class ThemeChanger {
  private storage = inject(StorageService);
  private themeSignal = signal<Theme>(this.storage.get(StorageKey.Theme) as Theme || Theme.Light);

  readonly currentTheme = this.themeSignal.asReadonly();

  constructor() {
    effect(() => {
      const theme = this.themeSignal();

      if (theme === Theme.Dark) {
        document.documentElement.classList.add('dark-mode');
      } else {
        document.documentElement.classList.remove('dark-mode');
      }

      this.storage.set(StorageKey.Theme, theme);
    });
  }

  public toggleTheme() {
    this.themeSignal.update(v => v === Theme.Light ? Theme.Dark : Theme.Light);
  }

  public loadStoredTheme() {
    const theme = this.storage.get(StorageKey.Theme) as Theme || Theme.Light;
    console.log(theme);
    
    this.themeSignal.set(theme);
  }
}

export enum Theme {
  Light = 'light',
  Dark = 'dark'
}