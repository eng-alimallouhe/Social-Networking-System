import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../../shared/constants/api-routes/content-management-api.routes';
import { Result } from '../../../../shared/contracts/result';
import { CommunitySummaryDto } from '../../communities/contracts/community-summary.dto';

@Injectable({
    providedIn: 'root'
})
export class CommunityTrendingService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    getTrendingCommunities(count: number = 10): Observable<Result<CommunitySummaryDto[]>> {
        const params = new HttpParams().set('count', count.toString());

        return this.http.get<Result<CommunitySummaryDto[]>>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.TrendingCommunities}`,
            { params }
        );
    }
}
