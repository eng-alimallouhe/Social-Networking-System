import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { RESUMES_API_ROUTES } from '../../../shared/constants/api-routes/resumes-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ResumeEducationDto } from '../contracts/resume-education.dto';
import { AddResumeEducationCommand } from '../contracts/add-resume-education.command';
import { UpdateResumeEducationCommand } from '../contracts/update-resume-education.command';

@Injectable({
    providedIn: 'root',
})
export class ResumeEducationsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getResumeEducations(resumeId: string): Observable<Result<ResumeEducationDto[]>> {
        return this.http.get<Result<ResumeEducationDto[]>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Educations(resumeId)}`
        );
    }

    addResumeEducation(resumeId: string, command: AddResumeEducationCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Educations(resumeId)}`,
            command
        );
    }

    updateResumeEducation(
        resumeId: string,
        educationId: string,
        command: UpdateResumeEducationCommand
    ): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.EducationById(resumeId, educationId)}`,
            command
        );
    }

    deleteResumeEducation(resumeId: string, educationId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.EducationById(resumeId, educationId)}`
        );
    }
}
