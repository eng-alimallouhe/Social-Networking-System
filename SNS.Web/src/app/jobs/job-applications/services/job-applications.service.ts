import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { JOBS_API_ROUTES } from '../../../shared/constants/api-routes/jobs-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { ApplicationStatus } from '../../enums/application-status.enum';
import { CreateJobApplicationCommand } from '../contracts/create-job-application.command';
import { JobApplicationDetailsDto } from '../contracts/job-application-details.dto';
import { JobApplicationSummaryDto } from '../contracts/job-application-summary.dto';
import { UpdateJobApplicationStatusRequest } from '../contracts/update-job-application-status.request';

@Injectable({
    providedIn: 'root',
})
export class JobApplicationsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    createApplication(command: CreateJobApplicationCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobApplications}`,
            command
        );
    }

    getApplicationById(applicationId: string): Observable<Result<JobApplicationDetailsDto>> {
        return this.http.get<Result<JobApplicationDetailsDto>>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobApplicationById(applicationId)}`
        );
    }

    getMyApplications(
        status?: ApplicationStatus,
        pageSize: number = 10,
        currentPage: number = 1
    ): Observable<Result<Paged<JobApplicationSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (status) {
            params = params.set('status', status);
        }

        return this.http.get<Result<Paged<JobApplicationSummaryDto>>>(
            `${this.baseUrl}${JOBS_API_ROUTES.MyJobApplications}`,
            { params }
        );
    }

    getJobApplications(
        jobId: string,
        companyId?: string,
        status?: ApplicationStatus,
        pageSize: number = 10,
        currentPage: number = 1
    ): Observable<Result<Paged<JobApplicationSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        if (companyId) {
            params = params.set('companyId', companyId);
        }

        if (status) {
            params = params.set('status', status);
        }

        return this.http.get<Result<Paged<JobApplicationSummaryDto>>>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobApplicationsByJob(jobId)}`,
            { params }
        );
    }

    withdrawApplication(applicationId: string): Observable<Result> {
        return this.http.patch<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.WithdrawJobApplication(applicationId)}`,
            {}
        );
    }

    updateApplicationStatus(
        applicationId: string,
        request: UpdateJobApplicationStatusRequest
    ): Observable<Result> {
        return this.http.patch<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.ChangeJobApplicationStatus(applicationId)}`,
            request
        );
    }
}
