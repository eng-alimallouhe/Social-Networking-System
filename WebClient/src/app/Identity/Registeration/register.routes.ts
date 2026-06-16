import { Routes } from "@angular/router";

export const registerRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import('./Components/register/register').then(m => m.Register)
    },
    {
        path: 'activate-account',
        loadComponent: () => import('./Components/activate-account/activate-account').then(m => m.ActivateAccount)
    }
];