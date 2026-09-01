import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { JOBS_API_ROUTES } from '../../../shared/constants/api-routes/jobs-api.routes';
import { Paged } from '../../../shared/contracts/paged';
import { Result } from '../../../shared/contracts/result';
import { CompanyCreateRequestDetailsDto } from '../contracts/company-create-request-details.dto';
import { CompanyCreateRequestSummaryDto } from '../contracts/company-create-request-summary.dto';
import { CreateCompanyCreateRequestCommand } from '../contracts/create-company-create-request.command';
import { ReviewCompanyCreateRequestRequest } from '../contracts/review-company-create-request.request';

@Injectable({
    providedIn: 'root',
})
export class CompanyCreateRequestsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    createRequest(command: CreateCompanyCreateRequestCommand): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyCreateRequests}`,
            command
        );
    }

    getRequestById(requestId: string): Observable<Result<CompanyCreateRequestDetailsDto>> {
        return this.http.get<Result<CompanyCreateRequestDetailsDto>>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyCreateRequestById(requestId)}`
        );
    }

    getMyRequests(): Observable<Result<CompanyCreateRequestSummaryDto[]>> {
        return this.http.get<Result<CompanyCreateRequestSummaryDto[]>>(
            `${this.baseUrl}${JOBS_API_ROUTES.MyCompanyCreateRequests}`
        );
    }

    getPendingRequests(
        pageSize: number = 10,
        currentPage: number = 1
    ): Observable<Result<Paged<CompanyCreateRequestSummaryDto>>> {
        const params = new HttpParams()
            .set('pageSize', pageSize.toString())
            .set('currentPage', currentPage.toString());

        return this.http.get<Result<Paged<CompanyCreateRequestSummaryDto>>>(
            `${this.baseUrl}${JOBS_API_ROUTES.PendingCompanyCreateRequests}`,
            { params }
        );
    }

    cancelRequest(requestId: string): Observable<Result> {
        return this.http.patch<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.CancelCompanyCreateRequest(requestId)}`,
            {}
        );
    }

    approveRequest(requestId: string, reviewNote?: string): Observable<Result<string>> {
        const payload: ReviewCompanyCreateRequestRequest = { reviewNote };
        return this.http.post<Result<string>>(
            `${this.baseUrl}${JOBS_API_ROUTES.ApproveCompanyCreateRequest(requestId)}`,
            payload
        );
    }

    rejectRequest(requestId: string, reviewNote?: string): Observable<Result> {
        const payload: ReviewCompanyCreateRequestRequest = { reviewNote };
        return this.http.post<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.RejectCompanyCreateRequest(requestId)}`,
            payload
        );
    }
}
