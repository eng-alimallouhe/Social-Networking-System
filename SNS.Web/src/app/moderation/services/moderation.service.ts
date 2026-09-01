import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ViolationReason } from "../enums/violation-reason.enum";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment.development";
import { IDENTITY_API_ROUTES } from "../../shared/constants/api-routes/identity-api.routes";
import { Result } from "../../shared/contracts/result";

@Injectable({
    providedIn: 'root'
})
export class ModerationService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}${IDENTITY_API_ROUTES.Moderation}`

    reportPost(postId: string, reason: ViolationReason, details: string): Observable<Result<void>> {
        return this.http.post<Result<void>>(`${this.apiUrl}/${postId}/report`,
            {
                reason: reason,
                details: details
            });
    }
}