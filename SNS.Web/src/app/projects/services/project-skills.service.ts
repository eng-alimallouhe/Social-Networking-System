import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PROJECTS_API_ROUTES } from '../../shared/constants/api-routes/projects-api.routes';
import { Result } from '../../shared/contracts/result';
import { AddProjectSkillCommand } from '../contracts/project-skill.contracts';

@Injectable({
    providedIn: 'root'
})
export class ProjectSkillsService {
    private http = inject(HttpClient);

    addProjectSkill(projectId: string, command: AddProjectSkillCommand): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectSkills(projectId)}`;
        return this.http.post<Result>(url, command);
    }

    removeProjectSkill(projectId: string, projectSkillId: string): Observable<Result> {
        const url = `${environment.apiUrl}${PROJECTS_API_ROUTES.ProjectSkills(projectId)}/${projectSkillId}`;
        return this.http.delete<Result>(url);
    }
}
