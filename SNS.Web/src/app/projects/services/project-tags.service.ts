import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { AddProjectTagCommand } from '../contracts/project-tag.contracts';

@Injectable({
    providedIn: 'root'
})
export class ProjectTagsService {
    private http = inject(HttpClient);

    addProjectTag(projectId: string, command: AddProjectTagCommand): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectTags(projectId)}`;
        return this.http.post<Result>(url, command);
    }

    removeProjectTag(projectId: string, projectTagId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectTags(projectId)}/${projectTagId}`;
        return this.http.delete<Result>(url);
    }
}
