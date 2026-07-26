import { Routes } from "@angular/router";
import { AuthLayoutComponent } from "../shared/components/auth-layout/auth-layout.component";
import { SettingsLayoutComponent } from "./account-settings/components/settings-layout/settings-layout.component";

export const IdentityRoutes: Routes = [
    {
        path: 'auth',
        component: AuthLayoutComponent,
        children: [
            {
                path: 'login',
                loadComponent: () => import('./security-sessions/login/components/login-with-password/login-with-password.component').then(m => m.LoginWithPasswordComponent)
            },
            {
                path: 'login-with-authenticator-app',
                loadComponent: () => import('./security-sessions/login/components/login-with-authenticator/login-with-authenticator.component').then(m => m.LoginWithAuthenticatorComponent)
            },
            {
                path: 'login-with-passkey',
                loadComponent: () => import('./security-sessions/login/components/login-with-passkey/login-with-passkey.component').then(m => m.LoginWithPasskeyComponent)
            },
            {
                path: 'verify-otp',
                loadComponent: () => import('./security-sessions/login/components/verify-otp/verify-otp.component').then(m => m.VerifyOtpComponent)
            },
            {
                path: '',
                redirectTo: 'login',
                pathMatch: 'full'
            }
        ]
    },
    {
        path: 'account-settings',
        component: SettingsLayoutComponent,
        children: [
            {
                path: '',
                loadComponent: () => import('./account-settings/components/home-settings/home-settings.component').then(m => m.HomeSettingsComponent)
            },
            {
                path: 'personal-information',
                loadComponent: () => import('./account-settings/components/personal-information/personal-information.component').then(m => m.PersonalInformationComponent),
                children: [
                    {
                        path: 'change-username',
                        loadComponent: () => import('./account-settings/components/change-username/change-username.component').then(m => m.ChangeUsernameComponent)
                    }
                ]
            }
        ]
    },
    {
        path: '',
        redirectTo: 'auth/login',
        pathMatch: 'full'
    }
]