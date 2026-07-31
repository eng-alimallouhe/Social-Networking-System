import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'demo/role-switcher',
        pathMatch: 'full'
    },
    {
        path: '',
        loadChildren: () => import('./identity/routes/identity.routes').then(m => m.IdentityRoutes)
    },
    {
        path: 'demo',
        loadChildren: () => import('./demo/routes/demo.routes').then(m => m.DemoRoutes)
    },
    {
        path: '**',
        redirectTo: 'auth/login'
    }
];
