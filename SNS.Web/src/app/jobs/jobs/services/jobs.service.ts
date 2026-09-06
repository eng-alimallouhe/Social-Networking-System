import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { JOBS_API_ROUTES } from '../../../shared/constants/api-routes/jobs-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { CreateJobCommand } from '../contracts/create-job.command';
import { JobDetailsDto } from '../contracts/job-details.dto';
import { JobSummaryDto } from '../contracts/job-summary.dto';
import { UpdateJobCommand } from '../contracts/update-job.command';

@Injectable({
    providedIn: 'root',
})
export class JobsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    createJob(command: CreateJobCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${JOBS_API_ROUTES.Jobs}`,
            command
        );
    }

    getJobById(jobId: string): Observable<Result<JobDetailsDto>> {
        return this.http.get<Result<JobDetailsDto>>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobById(jobId)}`
        );
    }

    getMyCompanyJobs(
        companyId?: string,
        pageSize: number = 10,
        currentPage: number = 1,
        includeClosed: boolean = true
    ): Observable<Result<Paged<JobSummaryDto>>> {
        let params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString())
            .set('includeClosed', includeClosed.toString());

        if (companyId) {
            params = params.set('companyId', companyId);
        }

        return this.http.get<Result<Paged<JobSummaryDto>>>(
            `${this.baseUrl}${JOBS_API_ROUTES.MyCompanyJobs}`,
            { params }
        );
    }

    getJobsByCompany(
        companyId: string,
        pageSize: number = 10,
        currentPage: number = 1,
        includeClosed: boolean = false
    ): Observable<Result<Paged<JobSummaryDto>>> {
        const params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString())
            .set('includeClosed', includeClosed.toString());

        return this.http.get<Result<Paged<JobSummaryDto>>>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobsByCompany(companyId)}`,
            { params }
        );
    }

    updateJob(jobId: string, command: UpdateJobCommand): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobById(jobId)}`,
            command
        );
    }

    deleteJob(jobId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.JobById(jobId)}`
        );
    }

    closeJob(jobId: string): Observable<Result> {
        return this.http.patch<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.CloseJob(jobId)}`,
            {}
        );
    }
}
