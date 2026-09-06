import { Routes } from "@angular/router";
import { ProjectDetails } from "../components/project-details/project-details";
import { ProjectEdit } from "../components/project-edit/project-edit";

export const PROJECTS_ROUTES: Routes = [
    {
        path: ":projectId",
        component: ProjectDetails
    },
    {
        path: ":projectId/edit",
        component: ProjectEdit
    }
];