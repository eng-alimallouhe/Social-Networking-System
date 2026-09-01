import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Result } from "../../../shared/contracts/result";
import { SuggestedUser } from "../contracts/suggested-user";
import { ProfileSummaryDto } from "../contracts/profile-summary.dto";

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
}