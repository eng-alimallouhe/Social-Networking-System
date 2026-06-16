import { inject, Injectable, signal, effect } from '@angular/core';
import { StorageKey, StorageService } from './storage.service';

@Injectable({
  providedIn: 'root',
})
export class ThemeChanger {
  private storage = inject(StorageService);
  private themeSignal = signal<string>(this.storage.get(StorageKey.Theme) || 'light');

  readonly currentTheme = this.themeSignal.asReadonly();

  constructor() {
    effect(() => {
      const theme = this.themeSignal();
      
      if (theme === 'dark') {
        document.documentElement.classList.add('dark_mode');
      } else {
        document.documentElement.classList.remove('dark_mode');
      }

      this.storage.set(StorageKey.Theme, theme);
    });
  }

  toggleTheme() {
    this.themeSignal.update(v => v === 'light' ? 'dark' : 'light');
  }
}