import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { RESUMES_API_ROUTES } from '../../../shared/constants/api-routes/resumes-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ResumeLanguageDto } from '../contracts/resume-language.dto';
import { AddResumeLanguageCommand } from '../contracts/add-resume-language.command';
import { UpdateResumeLanguageCommand } from '../contracts/update-resume-language.command';

@Injectable({
    providedIn: 'root',
})
export class ResumeLanguagesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getResumeLanguages(resumeId: string): Observable<Result<ResumeLanguageDto[]>> {
        return this.http.get<Result<ResumeLanguageDto[]>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Languages(resumeId)}`
        );
    }

    addResumeLanguage(resumeId: string, command: AddResumeLanguageCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Languages(resumeId)}`,
            command
        );
    }

    updateResumeLanguage(
        resumeId: string,
        languageId: string,
        command: UpdateResumeLanguageCommand
    ): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.LanguageById(resumeId, languageId)}`,
            command
        );
    }

    deleteResumeLanguage(resumeId: string, languageId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.LanguageById(resumeId, languageId)}`
        );
    }
}
