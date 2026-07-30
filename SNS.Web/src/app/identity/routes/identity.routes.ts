import { Routes } from "@angular/router";

export const IdentityRoutes: Routes = [
    {
        path: '',
        loadChildren: () => import('../security-sesstions/routes/security-sessions.routes').then(m => m.SecuritySessionsRoutes)
    },
    {
        path: '',
        loadChildren: () => import('../users/routes/users.routes').then(m => m.UsersRoutes)
    },
    {
        path: '',
        redirectTo: 'auth/login',
        pathMatch: 'full'
    }
];