import { Routes } from '@angular/router';

export const RegisterationRoutes: Routes = [
    {
        path: 'register',
        children: [
            {
                path: '',
                loadComponent: () => import('../components/register/register').then(m => m.Register)
            },
            {
                path: 'verify-account',
                loadComponent: () => import('../components/verify-account/verify-account').then(m => m.VerifyAccount)
            }
        ]
    }
];