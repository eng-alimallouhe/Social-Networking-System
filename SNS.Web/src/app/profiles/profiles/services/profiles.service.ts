import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CreateProfileRequest } from "../contracts/create-profile-request.dto";
import { ProfileDetailsDto } from "../contracts/profile-details.dto";
import { Result } from "../../../shared/contracts/result";

@Injectable({
    providedIn: 'root'
})
export class ProfilesService {
    private apiUrl = environment.apiUrl + 'profiles/profiles';
    private http = inject(HttpClient);

    public createProfile(profile: CreateProfileRequest): Observable<Result> {
        const formData = new FormData();
        formData.append('fullName', profile.fullName);
        formData.append('bio', profile.bio || '');
        formData.append('specialization', profile.specialization || '');
        if (profile.profilePicture) {
            formData.append('profilePicture', profile.profilePicture);
        }
        return this.http.post<Result>(this.apiUrl, formData);
    }

    public getProfileById(profileId: string): Observable<Result<ProfileDetailsDto>> {
        return this.http.get<Result<ProfileDetailsDto>>(`${this.apiUrl}/${profileId}`);
    }
}