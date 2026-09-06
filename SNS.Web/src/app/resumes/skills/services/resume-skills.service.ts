import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { RESUMES_API_ROUTES } from '../../../shared/constants/api-routes/resumes-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ResumeSkillDto } from '../contracts/resume-skill.dto';
import { AddResumeSkillCommand } from '../contracts/add-resume-skill.command';
import { UpdateResumeSkillCommand } from '../contracts/update-resume-skill.command';

@Injectable({
    providedIn: 'root',
})
export class ResumeSkillsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getResumeSkills(resumeId: string): Observable<Result<ResumeSkillDto[]>> {
        return this.http.get<Result<ResumeSkillDto[]>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Skills(resumeId)}`
        );
    }

    addResumeSkill(resumeId: string, command: AddResumeSkillCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Skills(resumeId)}`,
            command
        );
    }

    updateResumeSkill(
        resumeId: string,
        skillId: string,
        command: UpdateResumeSkillCommand
    ): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.SkillById(resumeId, skillId)}`,
            command
        );
    }

    deleteResumeSkill(resumeId: string, skillId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.SkillById(resumeId, skillId)}`
        );
    }
}
