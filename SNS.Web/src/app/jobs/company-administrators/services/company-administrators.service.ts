import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';
import { JOBS_API_ROUTES } from '../../../shared/constants/api-routes/jobs-api.routes';
import { Result } from '../../../shared/contracts/result';
import { CompanyRole } from '../../enums/company-role.enum';
import { AddCompanyAdministratorRequest } from '../contracts/add-company-administrator.request';
import { ChangeCompanyAdministratorRoleRequest } from '../contracts/change-company-administrator-role.request';
import { CompanyAdministratorDto } from '../contracts/company-administrator.dto';

@Injectable({
    providedIn: 'root',
})
export class CompanyAdministratorsService {
    private http = inject(HttpClient);
    private baseUrl = environment.apiUrl;

    getCompanyAdministrators(companyId: string): Observable<Result<CompanyAdministratorDto[]>> {
        return this.http.get<Result<CompanyAdministratorDto[]>>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyAdministrators(companyId)}`
        );
    }

    getMyCompanyAdministratorRole(companyId: string): Observable<Result<CompanyRole | null>> {
        return this.http.get<Result<CompanyRole | null>>(
            `${this.baseUrl}${JOBS_API_ROUTES.MyCompanyAdministratorRole(companyId)}`
        );
    }

    addAdministrator(
        companyId: string,
        request: AddCompanyAdministratorRequest
    ): Observable<Result<string>> {
        return this.http.post<Result<string>>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyAdministrators(companyId)}`,
            request
        );
    }

    removeAdministrator(companyId: string, targetProfileId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.CompanyAdministratorByProfile(companyId, targetProfileId)}`
        );
    }

    changeAdministratorRole(
        companyId: string,
        targetProfileId: string,
        request: ChangeCompanyAdministratorRoleRequest
    ): Observable<Result> {
        return this.http.patch<Result>(
            `${this.baseUrl}${JOBS_API_ROUTES.ChangeCompanyAdministratorRole(companyId, targetProfileId)}`,
            request
        );
    }
}
