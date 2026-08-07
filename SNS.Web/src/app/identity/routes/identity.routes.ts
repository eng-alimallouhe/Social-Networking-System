import { Routes } from "@angular/router";

export const IdentityRoutes: Routes = [
    {
        path: '',
        loadChildren: () => import('../security-sesstions/routes/security-sessions.routes').then(m => m.SecuritySessionsRoutes)
    },
    {
        path: '',
        loadChildren: () => import('../users/routes/users.routes').then(m => m.UsersRoutes)
    },
    {
        path: '',
        loadChildren: () => import('../security-settings/routes/account-settings.routes').then(m => m.AccountSettingsRoutes)
    },
    {
        path: '',
        loadChildren: () => import('./onboarding.routes').then(m => m.OnboardingRoutes)
    }
];