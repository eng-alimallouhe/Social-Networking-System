import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { RateProjectCommand } from '../contracts/project-rating.contracts';

@Injectable({
    providedIn: 'root'
})
export class ProjectRatingsService {
    private http = inject(HttpClient);

    rateProject(projectId: string, command: RateProjectCommand): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectRatings(projectId)}`;
        return this.http.post<Result>(url, command);
    }
}
