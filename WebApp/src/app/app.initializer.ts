import { APP_INITIALIZER } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';

export function initializeApp(translate: TranslateService) {
    return () => {
        translate.setDefaultLang('en');
        const lang = 'en';
        return firstValueFrom(translate.use(lang));
    };
}