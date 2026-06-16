import { Routes as NgRoutes } from "@angular/router";
import { FeedComponent } from "./posts/components/feed/feed.component";
import { PostComponent } from "./posts/components/post/post.component";

export const contentRoutes: NgRoutes = [
    {
        path: '',
        children: [
            {
                path: 'feed',
                component: FeedComponent,
            },
            {
                path: 'posts/:id',
                component: PostComponent,
            },
            {
                path: '',
                redirectTo: 'feed',
                pathMatch: 'full',
            },
        ],
    },
];