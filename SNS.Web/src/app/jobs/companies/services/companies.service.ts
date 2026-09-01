import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { JOBS_API_ROUTES } from '../../../shared/constants/api-routes/jobs-api.routes';
import { Result } from '../../../shared/contracts/result';
import { CompanyDetailsDto } from '../contracts/company-details.dto';
import { CompanySummaryDto } from '../contracts/company-summary.dto';
import { UpdateCompanyCommand } from '../contracts/update-company.command';

@Injectable({
    providedIn: 'root',
})
export class CompaniesService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getCompanyById(companyId: string): Observable<Result<CompanyDetailsDto>> {
        return this.http.get<Result<CompanyDetailsDto>>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyById(companyId)}`
        );
    }

    getMyCompanies(): Observable<Result<CompanySummaryDto[]>> {
        return this.http.get<Result<CompanySummaryDto[]>>(
            `${this.baseUrl}${JOBS_API_ROUTES.MyCompanies}`
        );
    }

    updateCompany(companyId: string, command: UpdateCompanyCommand): Observable<Result> {
        return this.http.put<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyById(companyId)}`,
            command
        );
    }

    deleteCompany(companyId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyById(companyId)}`
        );
    }
}
