import {
    HttpContextToken,
    HttpErrorResponse,
    HttpInterceptorFn,
    HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {
    BehaviorSubject,
    catchError,
    filter,
    finalize,
    switchMap,
    take,
    throwError
} from 'rxjs';

import { RequestInformationService } from '../services/request-information.service';
import { AuthenticationService } from '../services/authentication.service';
import { SessionManagementService } from '../../account-settings/security-sessions/session-management/services/session-management.service';


/**
 * Marks a request that has already gone through the refresh flow.
 *
 * This prevents:
 *
 * Request
 *   ↓ 401
 * Refresh
 *   ↓
 * Retry Request
 *   ↓ 401
 * Refresh again
 *   ↓
 * ...
 */
const HAS_RETRIED = new HttpContextToken<boolean>(() => false);


let isRefreshing = false;

const refreshTokenSubject =
    new BehaviorSubject<string | null>(null);


export const authInterceptor: HttpInterceptorFn = (req, next) => {

    const authenticationService = inject(AuthenticationService);
    const requestInformationService = inject(RequestInformationService);
    const sessionManagementService = inject(SessionManagementService);
    const router = inject(Router);


    const accessToken =
        authenticationService.getAccessToken();

    const deviceId =
        requestInformationService.getDeviceId();

    const deviceToken =
        requestInformationService.getDeviceToken();

    const fingerprint =
        requestInformationService.getFingerprintHash();


    const authReq = addAuthenticationHeaders(
        req,
        accessToken,
        deviceId,
        deviceToken,
        fingerprint
    );


    return next(authReq).pipe(

        catchError((error: HttpErrorResponse) => {

            /*
             * We only care about Unauthorized responses.
             */
            if (error.status !== 401) {
                return throwError(() => error);
            }


            /*
             * NEVER refresh the refresh-token request itself.
             *
             * Otherwise:
             *
             * refresh-tokens → 401
             *      ↓
             * refresh-tokens → 401
             *      ↓
             * refresh-tokens → 401
             *      ↓
             * infinite loop
             */
            if (isRefreshRequest(req)) {
                return handleAuthenticationFailure(
                    authenticationService,
                    router,
                    error
                );
            }


            /*
             * If this request already went through the refresh flow,
             * don't refresh again.
             *
             * This protects us from:
             *
             * original request → 401
             * refresh → success
             * retry → 401
             * refresh again ❌
             */
            if (req.context.get(HAS_RETRIED)) {
                return handleAuthenticationFailure(
                    authenticationService,
                    router,
                    error
                );
            }


            /*
             * Another request is already refreshing the token.
             *
             * Wait for the existing refresh operation instead of
             * starting another one.
             */
            if (isRefreshing) {

                return refreshTokenSubject.pipe(

                    filter(
                        (token): token is string =>
                            token !== null
                    ),

                    take(1),

                    switchMap((token) => {

                        const retryRequest =
                            createRetryRequest(
                                req,
                                token,
                                deviceId,
                                deviceToken,
                                fingerprint
                            );

                        return next(retryRequest);
                    })
                );
            }


            /*
             * This request becomes responsible for refreshing
             * the access token.
             */
            isRefreshing = true;

            refreshTokenSubject.next(null);


            return sessionManagementService
                .refreshTokens()

                .pipe(

                    switchMap((response) => {

                        const newAccessToken =
                            extractAccessToken(response);


                        /*
                         * Refresh succeeded but didn't return
                         * an access token.
                         */
                        if (!newAccessToken) {

                            return handleAuthenticationFailure(
                                authenticationService,
                                router
                            );
                        }


                        /*
                         * Store the new access token.
                         */
                        authenticationService.setAccessToken(
                            newAccessToken
                        );


                        /*
                         * Notify all requests waiting for
                         * the refresh operation.
                         */
                        refreshTokenSubject.next(
                            newAccessToken
                        );


                        /*
                         * Retry the original request.
                         */
                        const retryRequest =
                            createRetryRequest(
                                req,
                                newAccessToken,
                                deviceId,
                                deviceToken,
                                fingerprint
                            );


                        return next(retryRequest);
                    }),


                    catchError(
                        (refreshError: HttpErrorResponse) => {

                            return handleAuthenticationFailure(
                                authenticationService,
                                router,
                                refreshError
                            );
                        }
                    ),


                    finalize(() => {
                        isRefreshing = false;
                    })
                );
        })
    );
};


/**
 * Adds authentication and device information
 * to the request.
 */
function addAuthenticationHeaders(
    req: HttpRequest<unknown>,
    accessToken: string | null,
    deviceId: string | null,
    deviceToken: string | null,
    fingerprint: string | null
): HttpRequest<unknown> {

    const headers: Record<string, string> = {

        'X-Device-Id':
            deviceId ?? '',

        'X-Device-Token':
            deviceToken ?? '',

        'X-Fingerprint-Hash':
            fingerprint ?? ''
    };


    if (accessToken) {

        headers['Authorization'] =
            `Bearer ${accessToken}`;
    }


    return req.clone({
        setHeaders: headers
    });
}


/**
 * Creates a retry request after successful token refresh.
 *
 * HAS_RETRIED is set to true so this request cannot trigger
 * another refresh cycle.
 */
function createRetryRequest(
    req: HttpRequest<unknown>,
    accessToken: string,
    deviceId: string | null,
    deviceToken: string | null,
    fingerprint: string | null
): HttpRequest<unknown> {

    const request =
        addAuthenticationHeaders(
            req,
            accessToken,
            deviceId,
            deviceToken,
            fingerprint
        );


    return request.clone({
        context: req.context.set(
            HAS_RETRIED,
            true
        )
    });
}


/**
 * Detects the refresh-token endpoint.
 *
 * Adjust the URL if your actual endpoint differs.
 */
function isRefreshRequest(
    req: HttpRequest<unknown>
): boolean {

    return req.url.includes('/refresh-tokens');
}


/**
 * Extracts the access token from the refresh response.
 *
 * Supports:
 *
 * {
 *   accessToken: "..."
 * }
 *
 * {
 *   data: {
 *      accessToken: "..."
 *   }
 * }
 *
 * {
 *   value: {
 *      accessToken: "..."
 *   }
 * }
 */
function extractAccessToken(
    response: any
): string | null {

    return (
        response?.accessToken ??
        response?.data?.accessToken ??
        response?.value?.accessToken ??
        null
    );
}


/**
 * Handles final authentication failure.
 */
function handleAuthenticationFailure(
    authenticationService: AuthenticationService,
    router: Router,
    error?: HttpErrorResponse
) {

    isRefreshing = false;

    refreshTokenSubject.next(null);


    /*
     * Remove the invalid access token.
     */
    authenticationService.removeToken();


    /*
     * Don't navigate repeatedly if we're already
     * on the login page.
     */
    if (!router.url.startsWith('/auth/login')) {

        void router.navigate(
            ['/auth/login'],
            {
                queryParams: {
                    returnUrl: router.url
                }
            }
        );
    }


    return throwError(
        () =>
            error ??
            new HttpErrorResponse({
                status: 401,
                statusText: 'Unauthorized'
            })
    );
}