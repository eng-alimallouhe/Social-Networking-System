import { Routes } from "@angular/router";
import { Feed } from "../components/feed/feed";
import { Post } from "../components/post/post";

export const POSTS_ROUTES: Routes = [
    {
        path: '',
        component: Feed,
        children: [
            {
                path: 'post/:postId',
                loadComponent: () => import('../components/post-details/post-details').then(m => m.PostDetails)
            }
        ]
    },
    {
        path: 'single',
        component: Post
    }
];