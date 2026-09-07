import { Routes } from "@angular/router";

export const PROFILES_ROUTES: Routes = [
    {
        path: ':profileId',
        loadComponent: () => import('../components/profile-details/profile-details').then(m => m.ProfileDetails)
    },
    {
        path: ':profileId/followers',
        loadComponent: () => import('../../social-graph/components/followers/followers').then(m => m.Followers)
    },
    {
        path: ':profileId/following',
        loadComponent: () => import('../../social-graph/components/following/following').then(m => m.Following)
    },
    {
        path: ':profileId/settings',
        loadComponent: () => import('../components/profile-edit/profile-edit').then(m => m.ProfileEdit)
    },
    {
        path: ':profileId/edit',
        redirectTo: ':profileId/settings'
    }
];
