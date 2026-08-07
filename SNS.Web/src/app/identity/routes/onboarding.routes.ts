import { Routes } from '@angular/router';
import { OnboardingLayout } from '../../shared/components/layouts/onboarding-layout/onboarding-layout';

export const OnboardingRoutes: Routes = [
    {
        path: 'onboarding',
        component: OnboardingLayout,
        children: [
            {
                path: '',
                redirectTo: 'create-profile',
                pathMatch: 'full'
            },
            {
                path: 'create-profile',
                loadComponent: () =>
                    import('../../profiles/profiles/components/create-profile/create-profile')
                        .then(m => m.CreateProfile)
            },
            {
                path: 'follow-people',
                loadComponent: () =>
                    import('../../profiles/social-graph/components/follow-people/follow-people')
                        .then(m => m.FollowPeople)
            }
        ]
    }
];
