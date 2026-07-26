import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './shared/components/auth-layout/auth-layout.component';
import { SettingsLayoutComponent } from './identity/account-settings/components/settings-layout/settings-layout.component';

export const routes: Routes = [
    {
        path: '',
        loadChildren: () => import('./identity/identity.routes').then(m => m.IdentityRoutes)
    }
];
