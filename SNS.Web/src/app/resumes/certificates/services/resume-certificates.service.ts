import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { RESUMES_API_ROUTES } from '../../../shared/constants/api-routes/resumes-api.routes';
import { Result } from '../../../shared/contracts/result';
import { ResumeCertificateDto } from '../contracts/resume-certificate.dto';
import { AddResumeCertificateCommand } from '../contracts/add-resume-certificate.command';
import { UpdateResumeCertificateCommand } from '../contracts/update-resume-certificate.command';

@Injectable({
    providedIn: 'root',
})
export class ResumeCertificatesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getResumeCertificates(resumeId: string): Observable<Result<ResumeCertificateDto[]>> {
        return this.http.get<Result<ResumeCertificateDto[]>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Certificates(resumeId)}`
        );
    }

    addResumeCertificate(resumeId: string, command: AddResumeCertificateCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${RESUMES_API_ROUTES.Certificates(resumeId)}`,
            command
        );
    }

    updateResumeCertificate(
        resumeId: string,
        certificateId: string,
        command: UpdateResumeCertificateCommand
    ): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.CertificateById(resumeId, certificateId)}`,
            command
        );
    }

    deleteResumeCertificate(resumeId: string, certificateId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${RESUMES_API_ROUTES.CertificateById(resumeId, certificateId)}`
        );
    }
}
