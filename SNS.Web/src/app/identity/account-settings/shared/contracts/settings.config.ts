import { LucideIdCard, LucideFingerprintPattern, LucideKeyRound, LucideShieldPlus, LucideArchive, LucideHouse } from '@lucide/angular';

export interface SettingEntry {
    id: string;
    titleKey: string;
    descriptionKey: string;
    categoryKey: string;
    keywords: string[];
    route: string;
    anchor?: string;
    icon: any;
    iconClass?: string;
}

export const SETTINGS_CONFIG: SettingEntry[] = [
    {
        id: 'account-home',
        titleKey: 'App.Layout.Settings.Home',
        descriptionKey: 'App.Layout.Settings.Home_Desc',
        categoryKey: 'App.Layout.Settings.Category.Account',
        keywords: ['Home', 'Overview', 'Profile', 'Picture', 'Email'],
        route: '/account-settings',
        icon: LucideHouse,
        iconClass: 'home-icon-style'
    },
    {
        id: 'personal-info',
        titleKey: 'App.Layout.Settings.Personal_Info',
        descriptionKey: 'App.Layout.Settings.Personal_Info_Desc',
        categoryKey: 'App.Layout.Settings.Category.Account',
        keywords: ['Personal', 'Information', 'Profile', 'Details', 'Name', 'Date of birth'],
        route: '/account-settings/personal-information',
        icon: LucideIdCard,
        iconClass: 'personal-info-icon-style'
    },
    {
        id: 'security-settings',
        titleKey: 'App.Layout.Settings.Security_Settings',
        descriptionKey: 'App.Layout.Settings.Security_Settings_Desc',
        categoryKey: 'App.Layout.Settings.Category.Security',
        keywords: ['Security', 'Settings', '2FA', 'Two-Factor', 'Authenticator', 'App', 'Keys'],
        route: '/account-settings/security-settings',
        icon: LucideFingerprintPattern,
        iconClass: 'security-icon-style'
    },
    {
        id: 'sessions-signin',
        titleKey: 'App.Layout.Settings.Sessions',
        descriptionKey: 'App.Layout.Settings.Sessions_Desc',
        categoryKey: 'App.Layout.Settings.Category.Security',
        keywords: ['Sessions', 'Sign-in', 'Devices', 'Active', 'Log out'],
        route: '/account-settings/sessions',
        icon: LucideShieldPlus,
        iconClass: 'security-icon-style'
    },
    {
        id: 'account-archive',
        titleKey: 'App.Layout.Settings.Archive',
        descriptionKey: 'App.Layout.Settings.Archive_Desc',
        categoryKey: 'App.Layout.Settings.Category.Data',
        keywords: ['Archive', 'Download', 'Data', 'Backup', 'Export', 'Delete'],
        route: '/account-settings/archive',
        icon: LucideArchive,
        iconClass: 'archive-icon-style'
    }
];
