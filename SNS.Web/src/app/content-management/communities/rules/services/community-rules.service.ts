import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../../shared/constants/api-routes/content-management-api.routes';
import { Result } from '../../../../shared/contracts/result';
import { CommunityRuleDto } from '../contracts/community-rule.dto';
import { CreateCommunityRuleRequest } from '../contracts/create-community-rule.request';
import { UpdateCommunityRuleRequest } from '../contracts/update-community-rule.request';

@Injectable({
    providedIn: 'root'
})
export class CommunityRulesService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    getRules(communityId: string): Observable<Result<CommunityRuleDto[]>> {
        return this.http.get<Result<CommunityRuleDto[]>>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityRules(communityId)}`
        );
    }

    createRule(communityId: string, request: CreateCommunityRuleRequest): Observable<Result> {
        return this.http.post<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityRules(communityId)}`,
            request
        );
    }

    updateRule(communityId: string, ruleId: string, request: UpdateCommunityRuleRequest): Observable<Result> {
        return this.http.put<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityRuleById(communityId, ruleId)}`,
            request
        );
    }

    deleteRule(communityId: string, ruleId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityRuleById(communityId, ruleId)}`
        );
    }
}
