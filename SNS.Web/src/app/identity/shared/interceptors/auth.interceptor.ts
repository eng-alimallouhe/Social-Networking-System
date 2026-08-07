import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';
import { RequestInformationService } from '../services/request-information.service';
import { LoginService } from '../../security-sesstions/login/services/login.service';
import { RefreshTokenService } from '../services/refresh-token.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const tokenService = inject(TokenService);
    const reqInfoService = inject(RequestInformationService);
    const router = inject(Router);
    const refreshTokenService = inject(RefreshTokenService);

    let accessToken = tokenService.getAccessToken();
    const deviceId = reqInfoService.getDeviceId();
    const deviceToken = reqInfoService.getDeviceToken();
    const fingerprint = reqInfoService.getFingerprintHash();


    const authReq = req.clone({
        setHeaders: {
            'Authorization': accessToken ? `Bearer ${accessToken}` : '',
            'X-Device-Id': deviceId || '',
            'X-Device-Token': deviceToken || '',
            'X-Fingerprint-Hash': fingerprint || ''
        }
    });

    return next(authReq).pipe(

        catchError(error => {

            if (error.status !== 401) {
                return throwError(() => error);
            }

            return refreshTokenService.refresh().pipe(
                switchMap(() => {
                    const retry = req.clone({
                        setHeaders: {
                            Authorization:
                                `Bearer ${tokenService.getAccessToken()}`
                        }
                    });
                    return next(retry);
                }),
                catchError(() => {
                    tokenService.removeToken();
                    router.navigate(['/auth/login']);
                    return throwError(() => error);
                })
            );
        })
    );

};