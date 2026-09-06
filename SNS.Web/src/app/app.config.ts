import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';

import { routes } from './app.routes';
import { HttpClient, provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideTranslateService, TranslateLoader } from '@ngx-translate/core';
import { authInterceptor } from './identity/shared/interceptors/auth.interceptor';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../environments/environment.development';
import { errorInterceptor } from './shared/interceptors/error-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(
      routes
    ),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor, errorInterceptor])),
    provideTranslateService({
      fallbackLang: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    })
  ]
};


export class CustomTranslateHttpLoader implements TranslateLoader {
  private translationCache = new Map<string, Observable<any>>();

  constructor(
    private http: HttpClient,
    private prefix: string = './assets/i18n/',
    private suffix: string = '.json'
  ) { }

  public getTranslation(lang: string): Observable<any> {
    if (!this.translationCache.has(lang)) {
      const request$ = this.http
        .get(`${this.prefix}${lang}${this.suffix}?v=${environment.appVersion}`)
        .pipe(shareReplay(1));

      this.translationCache.set(lang, request$);
    }
    return this.translationCache.get(lang)!;
  }
}

export function HttpLoaderFactory(http: HttpClient) {
  return new CustomTranslateHttpLoader(http, './assets/i18n/', '.json');
}