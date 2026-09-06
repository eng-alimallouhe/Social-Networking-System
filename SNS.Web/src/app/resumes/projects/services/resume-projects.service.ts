import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { RESUMES_API_ROUTES } from '../../../shared/constants/api-routes/resumes-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ResumeProjectDto } from '../contracts/resume-project.dto';
import { AddResumeProjectCommand } from '../contracts/add-resume-project.command';

@Injectable({
    providedIn: 'root',
})
export class ResumeProjectsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getResumeProjects(resumeId: string): Observable<Result<ResumeProjectDto[]>> {
        return this.http.get<Result<ResumeProjectDto[]>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Projects(resumeId)}`
        );
    }

    addResumeProject(resumeId: string, command: AddResumeProjectCommand): Observable<Result> {
        return this.http.post<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Projects(resumeId)}`,
            command
        );
    }

    removeResumeProject(resumeId: string, projectId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.ProjectById(resumeId, projectId)}`
        );
    }
}
