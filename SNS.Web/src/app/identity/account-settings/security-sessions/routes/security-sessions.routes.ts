import { Routes } from "@angular/router";
import { authGuard } from "../../../../shared/guards/auth-guard";
import { guestGuard } from "../../../../shared/guards/guest-guard";

export const SecuritySessionsRoutes: Routes = [
    {
        path: 'auth',
        loadComponent: () => import('../../../shared/layout/components/auth-layout/auth-layout').then(m => m.AuthLayout),
        canActivateChild: [guestGuard],
        children: [
            {
                path: '',
                loadChildren: () => import('../login/routes/login.routes').then(m => m.LoginRoutes)
            }
        ]
    },
    {
        path: 'sessions',
        canActivate: [authGuard],
        loadChildren: () => import('../session-management/routes/session-management.routes').then(m => m.SessionManagementRoutes)
    }
];