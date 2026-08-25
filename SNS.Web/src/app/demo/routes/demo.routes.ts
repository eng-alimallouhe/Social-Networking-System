import { Routes } from "@angular/router";
import { DemoDashboard } from "../components/demo-dashboard/demo-dashboard";
import { DemoLayout } from "../components/demo-layout/demo-layout";
import { RoleSwitcher } from "../components/role-switcher/role-switcher";

export const DemoRoutes: Routes = [
    {
        path: '',
        component: DemoLayout,
        children: [
            {
                path: '',
                redirectTo: 'role-switcher',
                pathMatch: 'full',
            },
            {
                path: 'dashboard',
                component: DemoDashboard
            },
            {
                path: 'role-switcher',
                component: RoleSwitcher
            }
        ]
    }
];