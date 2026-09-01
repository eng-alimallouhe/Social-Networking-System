import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';

@Injectable({
    providedIn: 'root'
})
export class SavedProjectsService {
    private http = inject(HttpClient);

    saveProject(projectId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.SavedProjects(projectId)}`;
        return this.http.post<Result>(url, {});
    }

    unsaveProject(projectId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.SavedProjects(projectId)}`;
        return this.http.delete<Result>(url);
    }
}
