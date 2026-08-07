import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../../identity/notifications/services/toast.service';
import { Result } from '../../shared/contracts/result';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  const TRANSLATION_PATH = '/assets/i18n/';

  if (req.url.includes(TRANSLATION_PATH)) {
    return next(req);
  }

  const toast = inject(ToastService);
  const translate = inject(TranslateService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const title = translate.instant('Status_Codes.Shared.Error_Title');
      console.log(req.url);
      console.log(error);

      if (isResult(error.error)) {

        const { category, code } = error.error.statusCode;

        const message = translate.instant(
          `Status_Codes.${category}.${code}`
        );

        toast.error(message, title);
      }
      else {

        toast.error(
          translate.instant('Status_Codes.Shared.Unexpected_Error'),
          title
        );
      }

      return throwError(() => error);
    })
  );
};

function isResult(value: unknown): value is Result {
  return !!value
    && typeof value === 'object'
    && 'statusCode' in value
    && 'isSuccess' in value;
}