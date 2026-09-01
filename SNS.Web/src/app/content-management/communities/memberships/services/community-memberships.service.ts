import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { CONTENT_MANAGEMENT_API_ROUTES } from '../../../../shared/constants/api-routes/content-management-api.routes';
import { Paged } from '../../../../shared/contracts/paged';
import { Result } from '../../../../shared/contracts/result';
import { CommunityRole } from '../../../../shared/contracts/community-role';
import { ChangeMemberRoleRequest } from '../contracts/change-member-role.request';
import { CommunityMemberDto } from '../contracts/community-member.dto';
import { JoinCommunityRequest } from '../contracts/join-community.request';
import { MembershipRequestDto } from '../contracts/membership-request.dto';
import { UserMembershipStatusDto } from '../contracts/user-membership-status.dto';

@Injectable({
    providedIn: 'root'
})
export class CommunityMembershipsService {
    private http = inject(HttpClient);
    private rootUrl = environment.apiUrl;

    joinCommunity(communityId: string, request?: JoinCommunityRequest): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.JoinCommunity(communityId)}`, request || {});
    }

    leaveCommunity(communityId: string): Observable<Result> {
        return this.http.post<Result>(`${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.LeaveCommunity(communityId)}`, {});
    }

    getCommunityMembers(
        communityId: string,
        role?: CommunityRole | null,
        page: number = 1,
        pageSize: number = 20
    ): Observable<Result<Paged<CommunityMemberDto>>> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (role) {
            params = params.set('role', role);
        }

        return this.http.get<Result<Paged<CommunityMemberDto>>>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.CommunityMembers(communityId)}`,
            { params }
        );
    }

    getMembershipRequests(
        communityId: string,
        page: number = 1,
        pageSize: number = 20
    ): Observable<Result<Paged<MembershipRequestDto>>> {
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<Result<Paged<MembershipRequestDto>>>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.MembershipRequests(communityId)}`,
            { params }
        );
    }

    approveMembershipRequest(communityId: string, requestId: string): Observable<Result> {
        return this.http.post<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.ApproveMembershipRequest(communityId, requestId)}`,
            {}
        );
    }

    rejectMembershipRequest(communityId: string, requestId: string): Observable<Result> {
        return this.http.post<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.RejectMembershipRequest(communityId, requestId)}`,
            {}
        );
    }

    removeMember(communityId: string, memberProfileId: string): Observable<Result> {
        return this.http.delete<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.RemoveCommunityMember(communityId, memberProfileId)}`
        );
    }

    changeMemberRole(communityId: string, memberProfileId: string, request: ChangeMemberRoleRequest): Observable<Result> {
        return this.http.put<Result>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.ChangeCommunityMemberRole(communityId, memberProfileId)}`,
            request
        );
    }

    getMyMembershipStatus(communityId: string): Observable<Result<UserMembershipStatusDto>> {
        return this.http.get<Result<UserMembershipStatusDto>>(
            `${this.rootUrl}${CONTENT_MANAGEMENT_API_ROUTES.MyCommunityMembershipStatus(communityId)}`
        );
    }
}
