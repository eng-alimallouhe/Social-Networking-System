import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Result } from "../../../shared/contracts/result";

@Injectable({
    providedIn: 'root'
})
export class FollowsService {
    private apiUrl = environment.apiUrl;
    private http = inject(HttpClient);

    public followProfile(profileId: string): Observable<Result> {
        return this.http.post<Result>(`${this.apiUrl}/follows/${profileId}`, {});
    }

    public unfollowProfile(profileId: string): Observable<Result> {
        return this.http.delete<Result>(`${this.apiUrl}/follows/${profileId}`, {});
    }
}