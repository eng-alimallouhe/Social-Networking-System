import { InjectionToken } from '@angular/core';
import { 
    LucideLogIn, 
    LucideUserPlus, 
    LucideCheckCircle, 
    LucideUsers, 
    LucideLayoutDashboard,
    LucideKeyRound,
    LucideSmartphone,
    LucideShieldCheck,
    LucideIdCard,
    LucideFingerprintPattern,
    LucideShieldPlus,
    LucideArchive,
    LucideHouse
} from '@lucide/angular';
import { DemoDataService } from './services/demo-data.service';

export interface DemoPage {
    titleKey: string;
    descriptionKey: string;
    route: string;
    icon: any;
    queryParams?: Record<string, string>;
    generateQueryParams?: (dataService: DemoDataService) => Record<string, string>;
}

export interface DemoSection {
    titleKey: string;
    pages: DemoPage[];
}

export const DEMO_CONFIG: DemoSection[] = [
    {
        titleKey: 'Demo.Dashboard.Sections.Authentication',
        pages: [
            {
                titleKey: 'Demo.Dashboard.Pages.Login',
                descriptionKey: 'Demo.Dashboard.Pages.Login_Desc',
                route: '/auth/login/password',
                icon: LucideLogIn
            },
            {
                titleKey: 'Demo.Dashboard.Pages.LoginPasskey',
                descriptionKey: 'Demo.Dashboard.Pages.LoginPasskey_Desc',
                route: '/auth/login/passkey',
                icon: LucideKeyRound,
                generateQueryParams: (data) => ({ ui: data.getDemoEmail() })
            },
            {
                titleKey: 'Demo.Dashboard.Pages.LoginAuthApp',
                descriptionKey: 'Demo.Dashboard.Pages.LoginAuthApp_Desc',
                route: '/auth/login/authenticator-app',
                icon: LucideSmartphone,
                generateQueryParams: (data) => ({ ui: data.getDemoEmail() })
            },
            {
                titleKey: 'Demo.Dashboard.Pages.VerifyOTP',
                descriptionKey: 'Demo.Dashboard.Pages.VerifyOTP_Desc',
                route: '/auth/login/verify-otp',
                icon: LucideShieldCheck,
                generateQueryParams: (data) => ({ 
                    uid: data.getDemoUserId(), 
                    'challenge-token': data.generateChallengeToken() 
                })
            }
        ]
    },
    {
        titleKey: 'Demo.Dashboard.Sections.Registration',
        pages: [
            {
                titleKey: 'Demo.Dashboard.Pages.Register',
                descriptionKey: 'Demo.Dashboard.Pages.Register_Desc',
                route: '/auth/register',
                icon: LucideUserPlus
            },
            {
                titleKey: 'Demo.Dashboard.Pages.VerifyAccount',
                descriptionKey: 'Demo.Dashboard.Pages.VerifyAccount_Desc',
                route: '/auth/register/verify-account',
                icon: LucideCheckCircle,
                generateQueryParams: (data) => ({ 
                    uid: data.getDemoUserId(), 
                    'challenge-token': data.generateChallengeToken() 
                })
            }
        ]
    },
    {
        titleKey: 'Demo.Dashboard.Sections.DemoPages',
        pages: [
            {
                titleKey: 'Demo.Dashboard.Pages.RoleSwitcher',
                descriptionKey: 'Demo.Dashboard.Pages.RoleSwitcher_Desc',
                route: '/demo/role-switcher',
                icon: LucideUsers
            },
            {
                titleKey: 'Demo.Dashboard.Pages.Dashboard',
                descriptionKey: 'Demo.Dashboard.Pages.Dashboard_Desc',
                route: '/demo/dashboard',
                icon: LucideLayoutDashboard
            }
        ]
    },
    {
        titleKey: 'Demo.Dashboard.Sections.AccountSettings',
        pages: [
            {
                titleKey: 'App.Layout.Settings.Home',
                descriptionKey: 'App.Layout.Settings.Home_Desc',
                route: '/account-settings',
                icon: LucideHouse
            },
            {
                titleKey: 'App.Layout.Settings.Personal_Info',
                descriptionKey: 'App.Layout.Settings.Personal_Info_Desc',
                route: '/account-settings/personal-information',
                icon: LucideIdCard
            },
            {
                titleKey: 'App.Layout.Settings.Security_Settings',
                descriptionKey: 'App.Layout.Settings.Security_Settings_Desc',
                route: '/account-settings/security-settings',
                icon: LucideFingerprintPattern
            },
            {
                titleKey: 'App.Layout.Settings.Sessions',
                descriptionKey: 'App.Layout.Settings.Sessions_Desc',
                route: '/account-settings/sessions',
                icon: LucideShieldPlus
            },
            {
                titleKey: 'App.Layout.Settings.Password_Management',
                descriptionKey: 'App.Layout.Settings.Password_Management_Desc',
                route: '/account-settings/password-management',
                icon: LucideKeyRound
            },
            {
                titleKey: 'App.Layout.Settings.Archive',
                descriptionKey: 'App.Layout.Settings.Archive_Desc',
                route: '/account-settings/archive',
                icon: LucideArchive
            }
        ]
    }
];

