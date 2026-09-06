import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { RESUMES_API_ROUTES } from '../../../shared/constants/api-routes/resumes-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ResumeExperienceDto } from '../contracts/resume-experience.dto';
import { AddResumeExperienceCommand } from '../contracts/add-resume-experience.command';
import { UpdateResumeExperienceCommand } from '../contracts/update-resume-experience.command';

@Injectable({
    providedIn: 'root',
})
export class ResumeExperiencesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getResumeExperiences(resumeId: string): Observable<Result<ResumeExperienceDto[]>> {
        return this.http.get<Result<ResumeExperienceDto[]>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Experiences(resumeId)}`
        );
    }

    addResumeExperience(resumeId: string, command: AddResumeExperienceCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Experiences(resumeId)}`,
            command
        );
    }

    updateResumeExperience(
        resumeId: string,
        experienceId: string,
        command: UpdateResumeExperienceCommand
    ): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.ExperienceById(resumeId, experienceId)}`,
            command
        );
    }

    deleteResumeExperience(resumeId: string, experienceId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.ExperienceById(resumeId, experienceId)}`
        );
    }
}
