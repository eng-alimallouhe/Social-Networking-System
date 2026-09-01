import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../../shared/constants/api-routes/content-management-api.routes';
import { Paged } from '../../../../shared/contracts/paged';
import { Result } from '../../../../shared/contracts/result';
import { CommunityDetailsDto } from '../contracts/community-details.dto';
import { CommunitySummaryDto } from '../contracts/community-summary.dto';
import { CreateCommunityCommand } from '../contracts/create-community.command';
import { UpdateCommunityCommand } from '../contracts/update-community.command';

@Injectable({
    providedIn: 'root'
})
export class CommunitiesService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    createCommunity(command: CreateCommunityCommand): Observable<Result> {
        const formData = new FormData();
        formData.append('name', command.name);
        formData.append('description', command.description);
        formData.append('rulesText', command.rulesText);
        formData.append('policy', command.policy);
        formData.append('type', command.type);

        if (command.logo) {
            formData.append('logo', command.logo, command.logo.name);
        }

        if (command.settings) {
            formData.append('settings.allowPostWithoutApproval', String(command.settings.allowPostWithoutApproval));
            formData.append('settings.allowInvitationsByMembers', String(command.settings.allowInvitationsByMembers));
            formData.append('settings.allowComments', String(command.settings.allowComments));
            formData.append('settings.allowMediaUpload', String(command.settings.allowMediaUpload));
        }

        if (command.rules && command.rules.length > 0) {
            command.rules.forEach((rule, index) => {
                formData.append(`rules[${index}].title`, rule.title);
                formData.append(`rules[${index}].description`, rule.description);
                formData.append(`rules[${index}].order`, String(rule.order));
            });
        }

        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.Communities}`, formData);
    }

    updateCommunity(id: string, command: UpdateCommunityCommand): Observable<Result> {
        const formData = new FormData();
        formData.append('name', command.name);
        formData.append('description', command.description);
        formData.append('rulesText', command.rulesText);
        formData.append('policy', command.policy);
        formData.append('type', command.type);
        formData.append('status', command.status);

        if (command.logo) {
            formData.append('logo', command.logo, command.logo.name);
        }

        return this.http.put<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityById(id)}`, formData);
    }

    deleteCommunity(id: string): Observable<Result> {
        return this.http.delete<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityById(id)}`);
    }

    getCommunityById(id: string): Observable<Result<CommunityDetailsDto>> {
        return this.http.get<Result<CommunityDetailsDto>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityById(id)}`);
    }

    getMyCommunities(page: number = 1, pageSize: number = 10): Observable<Result<Paged<CommunitySummaryDto>>> {
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<CommunitySummaryDto>>>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.MyCommunities}`, { params });
    }
}
