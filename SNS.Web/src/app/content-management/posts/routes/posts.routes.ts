import { Routes } from "@angular/router";
import { Post } from "../components/post/post";

export const POSTS_ROUTES: Routes = [
    {
        path: '',
        component: Post
    }
];