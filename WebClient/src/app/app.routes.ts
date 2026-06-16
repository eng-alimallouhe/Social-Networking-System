import { Routes } from '@angular/router';
import { AppLayout } from './shared/Components/app-layout/app-layout';
import { ProjectComponent } from './projects/components/project/project.component';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'auth',
        pathMatch: 'full'
    },
    {
        path: 'auth',
        loadChildren: () => import('./Identity/Shared/identity.routes').then(m => m.IdentityRoutes)
    },
    {
        path: 'app',
        component: AppLayout,
        children: [
            {
                path: '',
                loadChildren: () => import('./content/content.routes').then(m => m.contentRoutes)
            },
            {
                path: 'search',
                loadComponent: () => import('./search/components/search/search.component').then(m => m.SearchComponent)
            },
            {
                path: 'project',
                component: ProjectComponent
            },
            {
                path: '',
                redirectTo: 'content',
                pathMatch: 'full'
            }
        ]
    },
    {
        path: '**',
        redirectTo: 'auth',
        pathMatch: 'full'
    }
];