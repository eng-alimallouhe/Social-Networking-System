import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../../shared/constants/api-routes/content-management-api.routes';
import { Result } from '../../../../shared/contracts/result';
import { CommunitySettingsDto } from '../contracts/community-settings.dto';
import { UpdateCommunitySettingsRequest } from '../contracts/update-community-settings.request';

@Injectable({
    providedIn: 'root'
})
export class CommunitySettingsService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    getSettings(communityId: string): Observable<Result<CommunitySettingsDto>> {
        return this.http.get<Result<CommunitySettingsDto>>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunitySettings(communityId)}`
        );
    }

    updateSettings(communityId: string, request: UpdateCommunitySettingsRequest): Observable<Result> {
        return this.http.put<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunitySettings(communityId)}`,
            request
        );
    }
}
