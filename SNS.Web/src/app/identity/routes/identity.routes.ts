import { Routes } from "@angular/router";

export const IdentityRoutes: Routes = [
    {
        path: '',
        loadChildren: () => import('../account-settings/security-sessions/routes/security-sessions.routes').then(m => m.SecuritySessionsRoutes)
    },
    {
        path: '',
        loadChildren: () => import('../account-settings/routes/account-settings.routes').then(m => m.AccountSettingsRoutes)
    },
    {
        path: '',
        loadChildren: () => import('./onboarding.routes').then(m => m.OnboardingRoutes)
    }
];