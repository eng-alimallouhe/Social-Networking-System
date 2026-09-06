import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { JOBS_API_ROUTES } from '../../../shared/constants/api-routes/jobs-api.routes';
import { Result } from '../../../shared/contracts/result';
import { AddJobSkillRequest } from '../contracts/add-job-skill.request';
import { JobSkillDto } from '../contracts/job-skill.dto';

@Injectable({
    providedIn: 'root',
})
export class JobSkillsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getJobSkills(jobId: string): Observable<Result<JobSkillDto[]>> {
        return this.http.get<Result<JobSkillDto[]>>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobSkills(jobId)}`
        );
    }

    addJobSkill(jobId: string, request: AddJobSkillRequest): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobSkills(jobId)}`,
            request
        );
    }

    removeJobSkill(jobId: string, skillId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobSkillById(jobId, skillId)}`
        );
    }
}
