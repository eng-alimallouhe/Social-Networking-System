import { Routes } from "@angular/router";

export const AccountRecoveryRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/recover-account-by-security-code/recover-account-by-security-code').then(m => m.RecoverAccountBySecurityCode)
    }
]