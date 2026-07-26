import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';
import { RequestInformationService } from '../services/request-information.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const tokenService = inject(TokenService);
    const reqInfoService = inject(RequestInformationService);
    const router = inject(Router);

    const accessToken = tokenService.getAccessToken();
    const deviceId = reqInfoService.getDeviceId();
    const deviceToken = reqInfoService.getDeviceToken();
    const fingerprint = reqInfoService.getFingerprintHash();

    console.log(accessToken ?? "Token Are Not Founded");


    const authReq = req.clone({
        setHeaders: {
            'Authorization': accessToken ? `Bearer ${accessToken}` : '',
            'X-Device-Id': deviceId || '',
            'X-Device-Token': deviceToken || '',
            'X-Fingerprint-Hash': fingerprint || ''
        }
    });

    return next(authReq).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
                tokenService.removeToken();

                router.navigate(['/auth/login'], {
                    queryParams: { returnUrl: router.url }
                });
            }

            return throwError(() => error);
        })
    );
};