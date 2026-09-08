import { Routes } from '@angular/router';
import { SuggestedJobs } from '../../jobs/components/suggested-jobs/suggested-jobs';

export const COMPANIES_ROUTES: Routes = [
    {
        path: ':companyId',
        loadComponent: () => import('../components/company-details/company-details').then(m => m.CompanyDetails)
    }
];
