import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { forkJoin, map, Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { Paged } from '../../../../shared/contracts/paged';
import { Result } from '../../../../shared/contracts/result';
import {
    GetUserArchiveQuery,
    GetUserIdentityArchiveQuery,
    GetUserPasswordArchiveQuery,
    UserArchiveSummaryDto,
    UserIdentityArchiveSummaryDto,
    UserPasswordArchiveSummaryDto
} from '../contracts/archive-management.models';
import { IDENTITY_API_ROUTES } from '../../../../shared/constants/api-routes/identity-api.routes';

@Injectable({
    providedIn: 'root'
})
export class ArchiveManagementService {
    private http = inject(HttpClient);
    private baseUrl = `${environment.apiUrl}${IDENTITY_API_ROUTES.ArchiveManagement}`;

    public getUserArchive(query: GetUserArchiveQuery): Observable<Result<Paged<UserArchiveSummaryDto>>> {
        let params = new HttpParams()
            .set('CurrentPage', query.currentPage.toString())
            .set('PageSize', query.pageSize.toString());


        if (query.targetUserId) {
            params = params.set('TargetUserId', query.targetUserId);
        }

        return this.http.get<Result<Paged<UserArchiveSummaryDto>>>(`${this.baseUrl}/user-archive`, { params });
    }

    public getUserIdentityArchive(query: GetUserIdentityArchiveQuery): Observable<Result<Paged<UserIdentityArchiveSummaryDto>>> {
        let params = new HttpParams()
            .set('CurrentPage', query.currentPage.toString())
            .set('PageSize', query.pageSize.toString());

        if (query.targetUserId) {
            params = params.set('TargetUserId', query.targetUserId);
        }

        return this.http.get<Result<Paged<UserIdentityArchiveSummaryDto>>>(`${this.baseUrl}/user-identity-archive`, { params });
    }

    public getUserPasswordArchive(query: GetUserPasswordArchiveQuery): Observable<Result<Paged<UserPasswordArchiveSummaryDto>>> {
        let params = new HttpParams()
            .set('CurrentPage', query.currentPage.toString())
            .set('PageSize', query.pageSize.toString());

        if (query.targetUserId) {
            params = params.set('TargetUserId', query.targetUserId);
        }

        return this.http.get<Result<Paged<UserPasswordArchiveSummaryDto>>>(`${this.baseUrl}/user-password-archive`, { params });
    }

    public requestAccountDataExport(): Observable<Result<any>> {
        return this.http.post<Result<any>>(`${this.baseUrl}/export-account-data`, {});
    }

    getArchiveSummary(userId: string | null) {
        return forkJoin({
            account: this.getUserArchive({ currentPage: 1, pageSize: 3, targetUserId: userId ?? undefined }),
            identity: this.getUserIdentityArchive({ currentPage: 1, pageSize: 3, targetUserId: userId ?? undefined }),
            password: this.getUserPasswordArchive({ currentPage: 1, pageSize: 3, targetUserId: userId ?? undefined })
        }).pipe(
            map(res => ({ account: res.account.value?.items, identity: res.identity.value?.items, password: res.password.value?.items }))
        );
    }
}
