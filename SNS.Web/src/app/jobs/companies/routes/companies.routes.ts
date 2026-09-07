import { Routes } from '@angular/router';

export const COMPANIES_ROUTES: Routes = [
    {
        path: ':companyId',
        loadComponent: () => import('../components/company-details/company-details').then(m => m.CompanyDetails)
    }
];
