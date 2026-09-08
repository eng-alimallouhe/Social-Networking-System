import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CreateProfileRequest } from "../contracts/create-profile-request.dto";
import { ProfileDetailsDto } from "../contracts/profile-details.dto";
import { ProfileBaseDto } from "../contracts/profile-base.dto";
import { Result } from "../../../shared/contracts/result";

import { UpdateBasicInformationDto } from "../contracts/update-basic-information.dto";
import { UpdateSocialLinksDto } from "../contracts/update-social-links.dto";
import { AuthTokenDto } from "../../../identity/shared/contracts/auth-token.dto";

@Injectable({
    providedIn: 'root'
})
export class ProfilesService {
    private apiUrl = environment.apiUrl + 'profiles/profiles';
    private skillApiUrl = environment.apiUrl + 'profiles/ProfileSkill';
    private http = inject(HttpClient);

    public createProfile(profile: CreateProfileRequest): Observable<Result<AuthTokenDto>> {
        const formData = new FormData();
        formData.append('fullName', profile.fullName);
        formData.append('bio', profile.bio || '');
        formData.append('specialization', profile.specialization || '');
        if (profile.profilePicture) {
            formData.append('profilePicture', profile.profilePicture);
        }
        return this.http.post<Result<AuthTokenDto>>(this.apiUrl, formData);
    }

    public getProfileById(profileId: string): Observable<Result<ProfileDetailsDto>> {
        return this.http.get<Result<ProfileDetailsDto>>(`${this.apiUrl}/${profileId}`);
    }

    public getCurrentUserProfile(): Observable<Result<ProfileBaseDto>> {
        return this.http.get<Result<ProfileBaseDto>>(`${this.apiUrl}/base`);
    }

    public updateBasicInformation(dto: UpdateBasicInformationDto): Observable<Result> {
        return this.http.put<Result>(`${this.apiUrl}/basic-information`, dto);
    }

    public updateSocialLinks(dto: UpdateSocialLinksDto): Observable<Result> {
        return this.http.put<Result>(`${this.apiUrl}/social-links`, dto);
    }

    public updateProfilePicture(file: File): Observable<Result> {
        const formData = new FormData();
        formData.append('profilePicture', file);
        return this.http.put<Result>(`${this.apiUrl}/profile-picture`, formData);
    }

    public addSkillToProfile(skillId: string, proficiencyLevel: number = 1): Observable<Result> {
        return this.http.post<Result>(this.skillApiUrl, {
            skillId,
            proficiencyLevel
        });
    }

    public removeSkillFromProfile(skillId: string): Observable<Result> {
        return this.http.delete<Result>(this.skillApiUrl, {
            body: {
                skillId
            }
        });
    }
}
