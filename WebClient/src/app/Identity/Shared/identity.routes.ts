import { Routes } from "@angular/router";

export const IdentityRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/identity-layout/identity-layout').then(m => m.IdentityLayout),
        children: [
            {
                path: '',
                redirectTo: 'login',
                pathMatch: 'full'
            },
            {
                path: 'login',
                loadChildren: () => import('../Authentication/authentication.routes').then(m => m.AuthenticationRoutes)
            },
            {
                path: 'register',
                loadChildren: () => import('../Registeration/register.routes').then(m => m.registerRoutes)
            },
            {
                path: 'account-recovery',
                loadChildren: () => import('../account-recovery/account-recovery.routes').then(m => m.AccountRecoveryRoutes)
            },
            {
                path: 'account-assistance',
                loadComponent: () => import('./components/account-assistance/account-assistance').then(m => m.AccountAssistance)
            }
        ]
    }
];