import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment.development";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { Result } from "../../../shared/contracts/result";
import { Paged } from "../../../shared/contracts/paged";
import { SuggestedUser } from "../contracts/suggested-user";
import { ProfileSummaryDto } from "../contracts/profile-summary.dto";
import { ProfileFollowDto } from "../contracts/profile-follow.dto";

@Injectable({
    providedIn: 'root'
})
export class FollowsService {
    private apiUrl = environment.apiUrl + 'profiles/social-graph/Follows';
    private http = inject(HttpClient);

    public followProfile(profileId: string): Observable<Result> {
        return this.http.post<Result>(`${this.apiUrl}/${profileId}`, {});
    }

    public unfollowProfile(profileId: string): Observable<Result> {
        return this.http.delete<Result>(`${this.apiUrl}/${profileId}`, {});
    }

    public getFollowSuggestions(): Observable<Result<ProfileSummaryDto[]>> {
        return this.http.get<Result<ProfileSummaryDto[]>>(`${this.apiUrl}/follow-suggestions`);
    }

    public getSuggestedFollowings(): Observable<Result<SuggestedUser[]>> {
        return this.http.get<Result<SuggestedUser[]>>(`${this.apiUrl}/suggested-followings`);
    }

    public getProfileFollowers(
        profileId: string,
        currentPage: number = 1,
        pageSize: number = 10,
        searchTerm?: string
    ): Observable<Result<Paged<ProfileFollowDto>>> {
        let params = new HttpParams()
            .set('CurrentPage', currentPage.toString())
            .set('PageSize', pageSize.toString());

        if (searchTerm?.trim()) {
            params = params.set('SearchTerm', searchTerm.trim());
        }

        return this.http.get<Result<Paged<ProfileFollowDto>>>(`${this.apiUrl}/${profileId}/followers`, { params });
    }

    public getProfileFollowings(
        profileId: string,
        currentPage: number = 1,
        pageSize: number = 10,
        searchTerm?: string
    ): Observable<Result<Paged<ProfileFollowDto>>> {
        let params = new HttpParams()
            .set('CurrentPage', currentPage.toString())
            .set('PageSize', pageSize.toString());

        if (searchTerm?.trim()) {
            params = params.set('SearchTerm', searchTerm.trim());
        }

        return this.http.get<Result<Paged<ProfileFollowDto>>>(`${this.apiUrl}/${profileId}/followings`, { params });
    }
}