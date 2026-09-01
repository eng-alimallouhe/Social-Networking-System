import { Routes } from '@angular/router';
import { HomeLayout } from '../components/home-layout/home-layout';
export const HomeRoutes: Routes = [
    {
        path: '',
        component: HomeLayout,
        children: [
            {
                path: '',
                loadChildren: () => import('../../content-management/routes/content-management.routes').then(m => m.CONTENT_MANAGEMENT_ROUTES)
            },
            {
                path: 'search',
                loadChildren: () => import('../../search/routes/search.routes').then(m => m.SEARCH_ROUTES)
            },
            {
                path: 'projects',
                loadChildren: () => import('../../projects/routes/projects.routes').then(m => m.PROJECTS_ROUTES)
            },
            // More home routes can be added here
        ]
    }
];
