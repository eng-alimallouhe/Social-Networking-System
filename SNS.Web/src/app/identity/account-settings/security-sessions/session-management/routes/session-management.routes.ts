import { Routes } from "@angular/router";
import { SessionsAndDevicesComponent } from "../components/sessions-and-devices/sessions-and-devices";
import { AllSessionsComponent } from "../components/all-sessions/all-sessions";
import { AllDevicesComponent } from "../components/all-devices/all-devices";

export const SessionManagementRoutes: Routes = [
    {
        path: '',
        component: SessionsAndDevicesComponent
    },
    {
        path: 'all-sessions',
        component: AllSessionsComponent
    },
    {
        path: 'all-devices',
        component: AllDevicesComponent
    },
    {
        path: ':sessionId',
        loadComponent: () => import('../components/session-details/session-details').then(m => m.SessionDetailsComponent)
    }
];