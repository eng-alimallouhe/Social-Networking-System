import { Routes } from '@angular/router';
import { ServerErrorResponse } from './shared/components/responses/server-error-response/server-error-response';
import { NotFoundResponse } from './shared/components/responses/not-found-response/not-found-response';

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
        path: 'error',
        component: ServerErrorResponse
    },
    {
        path: 'not-found',
        component: NotFoundResponse
    },
    {
        path: '**',
        redirectTo: 'auth/login'
    }
];
