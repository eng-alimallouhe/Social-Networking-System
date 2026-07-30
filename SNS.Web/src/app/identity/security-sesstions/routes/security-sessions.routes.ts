import { Routes } from "@angular/router";

export const SecuritySessionsRoutes: Routes = [
    {
        path: 'auth',
        loadComponent: () => import('../../shared/layout/components/auth-layout/auth-layout').then(m => m.AuthLayout),
        children: [
            {
                path: '',
                loadChildren: () => import('../login/routes/login.routes').then(m => m.LoginRoutes)
            }
        ]
    },
    {
        path: 'sessions',
        loadChildren: () => import('../session-management/routes/session-management.routes').then(m => m.SessionManagementRoutes)
    }
];