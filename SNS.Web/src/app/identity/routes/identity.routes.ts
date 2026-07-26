import { Routes } from "@angular/router";

export const IdentityRoutes: Routes = [
    {
        path: '',
        loadChildren: () => import('../security-sesstions/routes/security-sessions.routes').then(m => m.SecuritySessionsRoutes)
    }
];