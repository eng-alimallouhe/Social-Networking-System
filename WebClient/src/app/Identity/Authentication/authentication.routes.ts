import { Routes } from '@angular/router';

export const AuthenticationRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('./Components/login/login').then(m => m.Login)
    },
    {
        path: 'login',
        loadComponent: () => import('./Components/login/login').then(m => m.Login)
    },
    {
        path: 'validate-two-factor-code',
        loadComponent: () => import('./Components/validate-two-factor-code/validate-two-factor-code').then(m => m.ValidateTwoFactorCode)
    },
    {
        path: 'account-recovery',
        loadComponent: () => import('../account-recovery/components/recover-account-by-security-code/recover-account-by-security-code').then(m => m.RecoverAccountBySecurityCode)
    }
]