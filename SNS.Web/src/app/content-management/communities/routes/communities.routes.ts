import { Routes } from '@angular/router';

export const COMMUNITIES_ROUTES: Routes = [
    {
        path: ':communityId',
        loadComponent: () => import('../communities/components/community-details/community-details').then(m => m.CommunityDetails)
    }
];
