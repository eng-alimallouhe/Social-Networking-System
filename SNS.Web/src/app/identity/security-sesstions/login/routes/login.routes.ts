import { Routes } from "@angular/router";

export const LoginRoutes: Routes = [
    {
        path: 'login',
        children: [
            {
                path: '',
                redirectTo: 'password',
                pathMatch: 'full'
            },
            {
                path: 'password',
                loadComponent: () => import('../components/login-with-password/login-with-password').then(m => m.LoginWithPassword)
            },
            {
                path: 'authenticator-app',
                loadComponent: () => import('../components/login-with-authenticator-app/login-with-authenticator-app').then(m => m.LoginWithAuthenticatorApp)
            },
            {
                path: 'passkey',
                loadComponent: () => import('../components/login-with-passkey/login-with-passkey').then(m => m.LoginWithPasskey)
            },
            {
                path: 'verify-otp',
                loadComponent: () => import('../components/verify-otp/verify-otp').then(m => m.VerifyOtp)
            }
        ]
    }
];