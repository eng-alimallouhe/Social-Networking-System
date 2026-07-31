import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';
import { RequestInformationService } from '../services/request-information.service';
import { LoginService } from '../../security-sesstions/login/services/login.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const tokenService = inject(TokenService);
    const loginService = inject(LoginService);
    const reqInfoService = inject(RequestInformationService);
    const router = inject(Router);

    let accessToken = tokenService.getAccessToken();
    const deviceId = reqInfoService.getDeviceId();
    const deviceToken = reqInfoService.getDeviceToken();
    const fingerprint = reqInfoService.getFingerprintHash();

    console.log(accessToken ?? "Token Are Not Founded");

    // Hardcoded access token for testing

    accessToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMmJjYTU5Ny03N2RiLTRmZTktODNiYi0wMmM4YjQ3MGY2M2EiLCJqdGkiOiI1ZjgzNzJjMi1lMmUwLTQ3ZGYtOTdiNi1kMzBkNjI0NTc3YjkiLCJzaWQiOiIxZTJkYmEzMy0yOTJhLTQ0NDgtOWEwZS1iNDk4MDA5YjFhYjYiLCJyb2xlIjoiVXNlciIsInByb2ZpbGVJZCI6ImYyYTM4OWIzLTU0N2UtNGQ5NC05NGQ0LWRiNDU0NmJmZDExMyIsIm5iZiI6MTc4NTQ4OTg4MywiZXhwIjoyMjU4ODc1NDgzLCJpYXQiOjE3ODU0ODk4ODMsImlzcyI6IlNOU0FQSSIsImF1ZCI6IlNOU0NsaWVudCJ9.Tn8Bb6Jb-REpfW9jlMANDFrZZQXpFSeOa20gfa3QFmM';

    console.log(accessToken);


    const authReq = req.clone({
        setHeaders: {
            'Authorization': accessToken ? `Bearer ${accessToken}` : '',
            'X-Device-Id': deviceId || '',
            'X-Device-Token': deviceToken || '',
            'X-Fingerprint-Hash': fingerprint || ''
        }
    });

    console.log(authReq.headers);


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