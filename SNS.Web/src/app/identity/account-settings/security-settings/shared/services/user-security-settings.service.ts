import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../../../environments/environment.development";
import { IDENTITY_API_ROUTES } from "../../../../../shared/constants/api-routes/identity-api.routes";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { UserSecurityDetailsDto } from "../contracts/user-security-details.dto";
import { Result } from "../../../../../shared/contracts/result";

const ACCOUNT_CONTROLLER = '/account';

@Injectable({
    providedIn: 'root'
})
export class UserSecuritySettingsService {
    private apiUrl = `${environment.apiUrl}${IDENTITY_API_ROUTES.SecuritySettings}`
    private http = inject(HttpClient);

    getUserSecuritySettings(): Observable<Result<UserSecurityDetailsDto>> {
        return this.http.get<Result<UserSecurityDetailsDto>>(`${this.apiUrl}/user-security-settings`);
    }
}