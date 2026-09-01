import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { AddProjectMilestoneCommand } from '../contracts/project-milestone.contracts';

@Injectable({
    providedIn: 'root'
})
export class ProjectMilestonesService {
    private http = inject(HttpClient);

    addProjectMilestone(projectId: string, command: AddProjectMilestoneCommand): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectMilestones(projectId)}`;
        return this.http.post<Result>(url, command);
    }

    deleteProjectMilestone(projectId: string, milestoneId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectMilestones(projectId)}/${milestoneId}`;
        return this.http.delete<Result>(url);
    }
}
