import { Routes } from "@angular/router";
import { Search } from "../components/search/search";

export const SEARCH_ROUTES: Routes = [
    {
        path: '',
        component: Search,
        children: [
            {
                path: 'post/:postId',
                loadComponent: () => import('../../content-management/posts/components/post-details/post-details').then(m => m.PostDetails)
            },
            {
                path: 'profile/:profileId',
                loadComponent: () => import('../../profiles/profiles/components/profile-details/profile-details').then(m => m.ProfileDetails)
            },
            {
                path: 'profile/:profileId/followers',
                loadComponent: () => import('../../profiles/social-graph/components/followers/followers').then(m => m.Followers)
            },
            {
                path: 'profile/:profileId/following',
                loadComponent: () => import('../../profiles/social-graph/components/following/following').then(m => m.Following)
            },
            {
                path: 'profile/:profileId/settings',
                loadComponent: () => import('../../profiles/profiles/components/profile-edit/profile-edit').then(m => m.ProfileEdit)
            },
            {
                path: 'profile/:profileId/edit',
                redirectTo: 'profile/:profileId/settings'
            },
            {
                path: 'project/:projectId',
                loadComponent: () => import('../../projects/components/project-details/project-details').then(m => m.ProjectDetails)
            },
            {
                path: 'community/:communityId',
                loadComponent: () => import('../../content-management/communities/communities/components/community-details/community-details').then(m => m.CommunityDetails)
            },
            {
                path: 'job/:jobId',
                loadComponent: () => import('../../jobs/jobs/components/job-details/job-details').then(m => m.JobDetails)
            },
            {
                path: 'company/:companyId',
                loadComponent: () => import('../../jobs/companies/components/company-details/company-details').then(m => m.CompanyDetails)
            },
            {
                path: 'problem/:problemId',
                loadComponent: () => import('../../discussions/problems/problems/components/problem-details/problem-details').then(m => m.ProblemDetails)
            }
        ]
    }
];