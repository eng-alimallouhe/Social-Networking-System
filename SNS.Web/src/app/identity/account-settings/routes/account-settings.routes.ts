import { Routes } from "@angular/router";
import { SettingsLayout } from "../shared/components/settings-layout/settings-layout";
export const AccountSettingsRoutes: Routes = [
    {
        path: 'account-settings',
        component: SettingsLayout,
        children: [
            {
                path: '',
                loadComponent: () => import('../shared/components/home-settings/home-settings').then(m => m.HomeSettings)
            },
            {
                path: 'personal-information',
                loadComponent: () => import('../users/user-management/components/personal-information/personal-information').then(m => m.PersonalInformation),
                children: [
                    {
                        path: 'change-username',
                        loadComponent: () => import('../users/user-management/components/change-username/change-username').then(m => m.ChangeUsername)
                    },
                    {
                        path: 'change-language',
                        loadComponent: () => import('../users/user-management/components/change-language/change-language').then(m => m.ChangeLanguage)
                    },
                    {
                        path: 'change-email',
                        loadComponent: () => import('../security-settings/email-change/components/initial-email-change/initial-email-change').then(m => m.InitialEmailChange)
                    },
                    {
                        path: 'verify-email-change',
                        loadComponent: () => import('../security-settings/email-change/components/verify-email-change/verify-email-change').then(m => m.VerifyEmailChange)
                    }
                ]
            },
            {
                path: 'security-settings',
                loadComponent: () => import('../security-settings/shared/components/user-security-settings/user-security-settings').then(m => m.UserSecuritySettings),
                children: [
                    {
                        path: 'enable-mfa',
                        loadComponent: () => import('../security-settings/mfa-management/components/enable-mfa/enable-mfa').then(m => m.EnableMfa)
                    },
                    {
                        path: 'link-authenticator',
                        loadComponent: () => import('../security-settings/mfa-management/components/authenticator-setup/authenticator-setup').then(m => m.AuthenticatorSetup)
                    },
                    {
                        path: 'verify-authenticator',
                        loadComponent: () => import('../security-settings/mfa-management/components/verify-authenticator/verify-authenticator').then(m => m.VerifyAuthenticator)
                    },
                    {
                        path: 'change-recovery-email',
                        loadComponent: () => import('../security-settings/mfa-management/components/change-recovery-email/change-recovery-email').then(m => m.ChangeRecoveryEmail)
                    },
                    {
                        path: 'verify-recovery-email',
                        loadComponent: () => import('../security-settings/mfa-management/components/verify-recovery-email/verify-recovery-email').then(m => m.VerifyRecoveryEmail)
                    },
                    {
                        path: 'recovery-codes',
                        loadComponent: () => import('../security-settings/recovery/components/recovery-codes/recovery-codes').then(m => m.RecoveryCodes)
                    },
                    {
                        path: 'passkeys',
                        loadComponent: () => import('../security-settings/mfa-management/components/user-passkeys/user-passkeys').then(m => m.UserPasskeys)
                    },
                    {
                        path: 'change-password',
                        loadComponent: () => import('../password-managements/components/change-password/change-password').then(m => m.ChangePassword)
                    }
                ]
            },
            {
                path: 'archive',
                loadComponent: () => import('../archive-management/components/archive-management/archive-management.component').then(m => m.ArchiveManagementComponent),
                children: [
                    {
                        path: 'account/:tuid',
                        loadComponent: () => import('../archive-management/components/account-archive/account-archive.component').then(m => m.AccountArchiveComponent)
                    },
                    {
                        path: 'identity/:tuid',
                        loadComponent: () => import('../archive-management/components/identity-archive/identity-archive.component').then(m => m.IdentityArchiveComponent)
                    },
                    {
                        path: 'password/:tuid',
                        loadComponent: () => import('../archive-management/components/password-archive/password-archive.component').then(m => m.PasswordArchiveComponent)
                    }
                ]
            },
            {
                path: 'sessions',
                loadChildren: () => import('../security-sessions/session-management/routes/session-management.routes').then(m => m.SessionManagementRoutes)
            },
            {
                path: '**',
                redirectTo: ''
            }
        ]
    }
]