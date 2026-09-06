import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { RESUMES_API_ROUTES } from '../../../shared/constants/api-routes/resumes-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ResumeSummaryDto } from '../contracts/resume-summary.dto';
import { ResumeDetailsDto } from '../contracts/resume-details.dto';
import { CreateResumeCommand } from '../contracts/create-resume.command';
import { UpdateResumeCommand } from '../contracts/update-resume.command';

@Injectable({
    providedIn: 'root',
})
export class ResumesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getMyResumes(): Observable<Result<ResumeSummaryDto[]>> {
        return this.http.get<Result<ResumeSummaryDto[]>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.MyResumes}`
        );
    }

    getResumeById(resumeId: string): Observable<Result<ResumeDetailsDto>> {
        return this.http.get<Result<ResumeDetailsDto>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.ResumeById(resumeId)}`
        );
    }

    createResume(command: CreateResumeCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Resumes}`,
            command
        );
    }

    updateResume(resumeId: string, command: UpdateResumeCommand): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.ResumeById(resumeId)}`,
            command
        );
    }

    deleteResume(resumeId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.ResumeById(resumeId)}`
        );
    }
}
