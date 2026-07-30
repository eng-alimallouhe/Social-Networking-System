import { Routes } from "@angular/router";

export const UsersRoutes: Routes = [
    {
        path: 'auth',
        loadComponent: () => import('../../shared/layout/components/auth-layout/auth-layout').then(m => m.AuthLayout),
        children: [
            {
                path: '',
                loadChildren: () => import('../registeration/routes/registeration.routes').then(m => m.RegisterationRoutes)
            }
        ]
    }
];