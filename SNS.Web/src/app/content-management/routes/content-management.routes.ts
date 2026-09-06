import { Routes } from "@angular/router";

export const CONTENT_MANAGEMENT_ROUTES: Routes = [
    {
        path: '',
        loadChildren: () => import('../posts/routes/posts.routes').then(m => m.POSTS_ROUTES)
    }
];