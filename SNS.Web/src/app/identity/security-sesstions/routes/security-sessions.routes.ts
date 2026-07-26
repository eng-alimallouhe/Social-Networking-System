import { Routes } from "@angular/router";

export const SecuritySessionsRoutes: Routes = [
    {
        path: 'auth',
        loadComponent: () => import('../../shared/layout/components/auth-layout/auth-layout').then(m => m.AuthLayout),
        children: [
            {
                path: 'login',
                loadComponent: () => import('../login/components/login-with-password/login-with-password').then(m => m.LoginWithPassword)
            },
            {
                path: 'login-with-authenticator-app',
                loadComponent: () => import('../login/components/login-with-authenticator-app/login-with-authenticator-app').then(m => m.LoginWithAuthenticatorApp)
            },
            {
                path: 'login-with-passkey',
                loadComponent: () => import('../login/components/login-with-passkey/login-with-passkey').then(m => m.LoginWithPasskey)
            },
            {
                path: 'verify-otp',
                loadComponent: () => import('../login/components/verify-otp/verify-otp').then(m => m.VerifyOtp)
            },
            {
                path: '',
                redirectTo: 'auth/login',
                pathMatch: 'full'
            }
        ]
    },
    {
        path: '',
        redirectTo: 'auth/login',
        pathMatch: 'full'
    }
];