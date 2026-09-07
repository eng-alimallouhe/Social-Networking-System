import { Routes } from '@angular/router';

export const CONTENT_MANAGEMENT_ROUTES: Routes = [
    {
        path: '',
        loadChildren: () => import('../posts/routes/posts.routes').then(m => m.POSTS_ROUTES)
    },
    {
        path: 'posts',
        loadChildren: () => import('../posts/routes/posts.routes').then(m => m.POSTS_ROUTES)
    },
    {
        path: 'problems',
        loadChildren: () => import('../../discussions/problems/problems/routes/problems.routes').then(m => m.PROBLEMS_ROUTES)
    },
    {
        path: 'discussion',
        loadChildren: () => import('../../discussions/problems/problems/routes/problems.routes').then(m => m.PROBLEMS_ROUTES)
    },
    {
        path: 'communities',
        loadChildren: () => import('../communities/routes/communities.routes').then(m => m.COMMUNITIES_ROUTES)
    },
    {
        path: 'create-post',
        loadComponent: () => import('../posts/components/create-post/create-post').then(m => m.CreatePost)
    },
    {
        path: 'create-problem',
        loadComponent: () => import('../../discussions/problems/problems/components/create-problem/create-problem').then(m => m.CreateProblem)
    },
    {
        path: 'create-project',
        loadComponent: () => import('../../projects/components/create-project/create-project').then(m => m.CreateProject)
    }
];