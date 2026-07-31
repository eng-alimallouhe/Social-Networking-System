import { Routes } from "@angular/router";
import { SettingsLayout } from "../components/settings-layout/settings-layout";

export const AccountSettingsRoutes: Routes = [
    {
        path: 'account-settings',
        component: SettingsLayout,
        children: [
            {
                path: '',
                loadComponent: () => import('../components/home-settings/home-settings').then(m => m.HomeSettings)
            },
            {
                path: 'personal-information',
                loadComponent: () => import('../components/personal-information/personal-information').then(m => m.PersonalInformation),
                children: [
                    {
                        path: 'change-username',
                        loadComponent: () => import('../components/change-username/change-username').then(m => m.ChangeUsername)
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