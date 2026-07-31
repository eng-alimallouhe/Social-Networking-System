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

    let accessToken = tokenService.getAccessToken();
    const deviceId = reqInfoService.getDeviceId();
    const deviceToken = reqInfoService.getDeviceToken();
    const fingerprint = reqInfoService.getFingerprintHash();



    // Hardcoded access token for testing
    accessToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMmJjYTU5Ny03N2RiLTRmZTktODNiYi0wMmM4YjQ3MGY2M2EiLCJqdGkiOiJjYzNkOTk3Yy1hMDFiLTQyYWEtYjlkNS1kYjE5Mjc5YTMwNmIiLCJzaWQiOiJkYzE1NDExNS02YTIwLTRjODUtYjAwMC1iNDk4MDBlYTRmOTIiLCJyb2xlIjoiVXNlciIsInByb2ZpbGVJZCI6ImYyYTM4OWIzLTU0N2UtNGQ5NC05NGQ0LWRiNDU0NmJmZDExMyIsIm5iZiI6MTc4NTUwNzE4NiwiZXhwIjoyMjU4ODkyNzg2LCJpYXQiOjE3ODU1MDcxODYsImlzcyI6IlNOU0FQSSIsImF1ZCI6IlNOU0NsaWVudCJ9.Ga2A1MOnN832DcVMNk5FynbqffRH8-3ucACejVifx0o';

    console.log(accessToken);

    const authReq = req.clone({
        setHeaders: {
            'Authorization': accessToken ? `Bearer ${accessToken}` : 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0N2I0NDI2Zi0xZDJhLTRlMDMtYmQxMS1iNDk3MDBlNjAwMjMiLCJqdGkiOiI0YmE4M2ViZi1iYzZiLTRhOWEtYWNkZi1hNTZiYzI1MDNkNjAiLCJzaWQiOiJiYzQ4NTg1ZC1lOGMzLTQ1YWMtYjM0Zi1iNDk3MDBlNzE2MTIiLCJyb2xlIjoiVXNlciIsInByb2ZpbGVJZCI6IiIsIm5iZiI6MTc4NTQyMDA4MSwiZXhwIjoyMjU4ODA1NjgxLCJpYXQiOjE3ODU0MjAwODEsImlzcyI6IlNOU0FQSSIsImF1ZCI6IlNOU0NsaWVudCJ9.OabhasIGyQ8sYlFA7SSUCdMRELpZ83OEnWwyGfTAzyE',
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