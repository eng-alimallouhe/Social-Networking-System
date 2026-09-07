import { Routes } from '@angular/router';
import { ServerErrorResponse } from './shared/components/responses/server-error-response/server-error-response';
import { NotFoundResponse } from './shared/components/responses/not-found-response/not-found-response';
import { Post } from './content-management/posts/components/post/post';
import { HomeLayout } from './home/components/home-layout/home-layout';

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
        path: 'post',
        component: Post
    },
    {
        path: 'home',
        loadChildren: () => import('./home/routes/home.routes').then(m => m.HomeRoutes)
    },
    {
        path: 'projects',
        component: HomeLayout,
        children: [
            {
                path: '',
                loadChildren: () => import('./projects/routes/projects.routes').then(m => m.PROJECTS_ROUTES)
            }
        ]
    },
    {
        path: 'problems',
        redirectTo: 'home/problems'
    },
    {
        path: 'problems/:problemId',
        redirectTo: 'home/problems/:problemId'
    },
    {
        path: 'communities',
        redirectTo: 'home/communities'
    },
    {
        path: 'communities/:communityId',
        redirectTo: 'home/communities/:communityId'
    },
    {
        path: 'companies/:companyId',
        redirectTo: 'home/companies/:companyId'
    },
    {
        path: 'discussions',
        redirectTo: 'home/discussion'
    },
    {
        path: 'discussions/problems/:problemId',
        redirectTo: 'home/problems/:problemId'
    },
    {
        path: 'profile/:profileId',
        redirectTo: 'home/search/profile/:profileId'
    },
    {
        path: 'profile/:profileId/followers',
        redirectTo: 'home/search/profile/:profileId/followers'
    },
    {
        path: 'profile/:profileId/following',
        redirectTo: 'home/search/profile/:profileId/following'
    },
    {
        path: 'profile/:profileId/settings',
        redirectTo: 'home/profiles/:profileId/settings'
    },
    {
        path: 'jobs/:jobId',
        redirectTo: 'home/search/job/:jobId'
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
