import { Routes } from '@angular/router';
import { ProblemsPage } from '../components/problems-page/problems-page';

export const PROBLEMS_ROUTES: Routes = [
    {
        path: '',
        component: ProblemsPage,
        children: [
            {
                path: ':problemId',
                loadComponent: () => import('../components/problem-details/problem-details').then(m => m.ProblemDetails)
            }
        ]
    }
];
