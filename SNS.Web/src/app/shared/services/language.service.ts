import { Injectable, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { SupportedLanguage } from '../contracts/supported-language.enum';
import { StorageKey, StorageService } from './storage.service';
import { catchError, map, Observable, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private translate = inject(TranslateService);
  private storageService = inject(StorageService);

  public currentLanguage = signal<SupportedLanguage>(SupportedLanguage.English);

  constructor() {
  }

  public loadStoredLanguage(): void {
    const storedLanguage = this.storageService.get(StorageKey.Language);

    let language: SupportedLanguage;

    if (storedLanguage) {
      language =
        storedLanguage === SupportedLanguage.Arabic.toString()
          ? SupportedLanguage.Arabic
          : SupportedLanguage.English;
    } else {
      language = navigator.language.startsWith('ar')
        ? SupportedLanguage.Arabic
        : SupportedLanguage.English;
    }

    this.setLanguage(language).subscribe();
  }

  public setLanguage(language: SupportedLanguage) {
    const currentLanguage = this.storageService.get(StorageKey.Language);
    this.storageService.set(StorageKey.Language, language.toString());
    const langCode =
      language === SupportedLanguage.Arabic ? 'ar' : 'en';
    const isRtl = language === SupportedLanguage.Arabic;

    return this.translate.use(langCode).pipe(
      tap(() => {
        document.documentElement.dir = isRtl ? 'rtl' : 'ltr';
        document.body.classList.toggle('rtl', isRtl);
        this.currentLanguage.set(language);
      }),
      map(() => void 0),
      catchError((err) => {
        return of(void 0);
      })
    );
  }
}
