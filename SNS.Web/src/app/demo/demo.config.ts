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
    LucideHouse,
    LucideUser,
    LucideUserCheck,
    LucideSettings,
    LucideBuilding,
    LucideBriefcase,
    LucideGlobe,
    LucideFileText,
    LucideCode,
    LucideFolderPlus
} from '@lucide/angular';
import { DemoDataService } from './services/demo-data.service';

export interface DemoPage {
    titleKey: string;
    descriptionKey: string;
    route: string;
    icon: any;
    queryParams?: Record<string, string>;
    generateQueryParams?: (dataService: DemoDataService) => Record<string, string>;
    generateRoute?: (dataService: DemoDataService) => string;
}

export interface DemoSection {
    titleKey: string;
    pages: DemoPage[];
}

export const DEMO_CONFIG: DemoSection[] = [
    {
        titleKey: 'Demo.Dashboard.Sections.Profiles',
        pages: [
            {
                titleKey: 'Demo.Dashboard.Pages.ProfileSettings',
                descriptionKey: 'Demo.Dashboard.Pages.ProfileSettings_Desc',
                route: '/home/profiles/4c0f91e8-55f1-444b-b430-3c82cf75d950/settings',
                generateRoute: (data) => `/home/profiles/${data.getDemoProfileId()}/settings`,
                icon: LucideSettings
            },
            {
                titleKey: 'Demo.Dashboard.Pages.ProfileDetails',
                descriptionKey: 'Demo.Dashboard.Pages.ProfileDetails_Desc',
                route: '/home/profiles/4c0f91e8-55f1-444b-b430-3c82cf75d950',
                generateRoute: (data) => `/home/profiles/${data.getDemoProfileId()}`,
                icon: LucideUser
            },
            {
                titleKey: 'Demo.Dashboard.Pages.Followers',
                descriptionKey: 'Demo.Dashboard.Pages.Followers_Desc',
                route: '/home/profiles/4c0f91e8-55f1-444b-b430-3c82cf75d950/followers',
                generateRoute: (data) => `/home/profiles/${data.getDemoProfileId()}/followers`,
                icon: LucideUsers
            },
            {
                titleKey: 'Demo.Dashboard.Pages.Following',
                descriptionKey: 'Demo.Dashboard.Pages.Following_Desc',
                route: '/home/profiles/4c0f91e8-55f1-444b-b430-3c82cf75d950/following',
                generateRoute: (data) => `/home/profiles/${data.getDemoProfileId()}/following`,
                icon: LucideUserCheck
            }
        ]
    },
    {
        titleKey: 'Demo.Dashboard.Sections.Companies',
        pages: [
            {
                titleKey: 'Demo.Dashboard.Pages.CompanyDetails',
                descriptionKey: 'Demo.Dashboard.Pages.CompanyDetails_Desc',
                route: '/home/companies/b1c37f36-38c6-41bf-9e78-5e2260bde7c5',
                generateRoute: (data) => `/home/companies/${data.getDemoCompanyId()}`,
                icon: LucideBuilding
            },
            {
                titleKey: 'Demo.Dashboard.Pages.JobDetails',
                descriptionKey: 'Demo.Dashboard.Pages.JobDetails_Desc',
                route: '/home/search/job/2edb6e7e-521b-4906-860a-e52867efbf05',
                generateRoute: (data) => `/home/search/job/${data.getDemoJobId()}`,
                icon: LucideBriefcase
            }
        ]
    },
    {
        titleKey: 'Demo.Dashboard.Sections.Communities',
        pages: [
            {
                titleKey: 'Demo.Dashboard.Pages.CommunityDetails',
                descriptionKey: 'Demo.Dashboard.Pages.CommunityDetails_Desc',
                route: '/home/communities/a8abb121-698e-4d16-ba9d-04d3056b347a',
                generateRoute: (data) => `/home/communities/${data.getDemoCommunityId()}`,
                icon: LucideGlobe
            }
        ]
    },
    {
        titleKey: 'Demo.Dashboard.Sections.ContentCreation',
        pages: [
            {
                titleKey: 'Demo.Dashboard.Pages.CreatePost',
                descriptionKey: 'Demo.Dashboard.Pages.CreatePost_Desc',
                route: '/home/create-post',
                icon: LucideFileText
            },
            {
                titleKey: 'Demo.Dashboard.Pages.CreateProblem',
                descriptionKey: 'Demo.Dashboard.Pages.CreateProblem_Desc',
                route: '/home/create-problem',
                icon: LucideCode
            },
            {
                titleKey: 'Demo.Dashboard.Pages.CreateProject',
                descriptionKey: 'Demo.Dashboard.Pages.CreateProject_Desc',
                route: '/home/create-project',
                icon: LucideFolderPlus
            },
            {
                titleKey: 'Demo.Dashboard.Pages.HomeFeed',
                descriptionKey: 'Demo.Dashboard.Pages.HomeFeed_Desc',
                route: '/home',
                icon: LucideHouse
            }
        ]
    },
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

