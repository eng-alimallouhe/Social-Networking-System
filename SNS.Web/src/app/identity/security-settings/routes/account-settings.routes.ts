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
                loadComponent: () => import('../personal-information/components/personal-information/personal-information').then(m => m.PersonalInformation),
                children: [
                    {
                        path: 'change-username',
                        loadComponent: () => import('../../users/user-management/components/change-username/change-username').then(m => m.ChangeUsername)
                    },
                    {
                        path: 'change-language',
                        loadComponent: () => import('../../users/user-management/components/change-language/change-language').then(m => m.ChangeLanguage)
                    },
                    {
                        path: 'change-email',
                        loadComponent: () => import('../email-change/components/initial-email-change/initial-email-change').then(m => m.InitialEmailChange)
                    },
                    {
                        path: 'verify-email-change',
                        loadComponent: () => import('../email-change/components/verify-email-change/verify-email-change').then(m => m.VerifyEmailChange)
                    }
                ]
            },
            {
                path: 'security-settings',
                children: [
                    {
                        path: '',
                        loadComponent: () => import('../user-security-settings/components/user-security-settings/user-security-settings').then(m => m.UserSecuritySettings)
                    },
                    {
                        path: 'enable-mfa',
                        loadComponent: () => import('../user-security-settings/components/enable-mfa/enable-mfa').then(m => m.EnableMfa)
                    },
                    {
                        path: 'link-authenticator',
                        loadComponent: () => import('../user-security-settings/components/authenticator-setup/authenticator-setup').then(m => m.AuthenticatorSetup)
                    },
                    {
                        path: 'verify-authenticator',
                        loadComponent: () => import('../user-security-settings/components/verify-authenticator/verify-authenticator').then(m => m.VerifyAuthenticator)
                    },
                    {
                        path: 'change-recovery-email',
                        loadComponent: () => import('../user-security-settings/components/change-recovery-email/change-recovery-email').then(m => m.ChangeRecoveryEmail)
                    },
                    {
                        path: 'verify-recovery-email',
                        loadComponent: () => import('../user-security-settings/components/verify-recovery-email/verify-recovery-email').then(m => m.VerifyRecoveryEmail)
                    },
                    {
                        path: 'recovery-codes',
                        loadComponent: () => import('../user-security-settings/components/recovery-codes/recovery-codes').then(m => m.RecoveryCodes)
                    },
                    {
                        path: 'passkeys',
                        loadComponent: () => import('../user-security-settings/components/user-passkeys/user-passkeys').then(m => m.UserPasskeys)
                    }
                ]
            },
            {
                path: '**',
                redirectTo: ''
            }
        ]
    }
]