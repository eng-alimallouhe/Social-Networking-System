import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { 
    AddProjectContributorCommand, 
    ChangeContributorStatusRequest 
} from '../contracts/project-contributor.contracts';

@Injectable({
    providedIn: 'root'
})
export class ProjectContributorsService {
    private http = inject(HttpClient);

    addProjectContributor(projectId: string, command: AddProjectContributorCommand): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectContributors(projectId)}`;
        return this.http.post<Result>(url, command);
    }

    changeContributorRequestStatus(projectId: string, request: ChangeContributorStatusRequest): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectContributors(projectId)}/status`;
        return this.http.put<Result>(url, request);
    }
}
