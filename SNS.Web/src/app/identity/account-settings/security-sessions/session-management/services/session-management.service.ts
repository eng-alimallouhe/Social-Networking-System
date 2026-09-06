import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { finalize, Observable } from 'rxjs';
import { environment } from '../../../../../../environments/environment.development';
import { Result } from '../../../../../shared/contracts/result';
import { UserActiveSessionsAndDevicesResult } from '../contracts/user-active-sessions-and-devices-result.dto';
import { LogOutFromSessionCommand } from '../contracts/logout-from-session.command';
import { AuthTokenDto } from '../../../../shared/contracts/auth-token.dto';
import { IDENTITY_API_ROUTES } from '../../../../../shared/constants/api-routes/identity-api.routes';
import { SessionDetailsDto } from '../contracts/session-details.dto';
import { SessionSummaryDto } from '../contracts/session-summary.dto';
import { Paged } from '../../../../../shared/contracts/paged';
import { AuthenticationService } from '../../../../shared/services/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class SessionManagementService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}${IDENTITY_API_ROUTES.SessionManagement}`;
  private authenticationService = inject(AuthenticationService);

  getUserActiveSessionsAndDevices(): Observable<Result<UserActiveSessionsAndDevicesResult>> {
    return this.http.get<Result<UserActiveSessionsAndDevicesResult>>(`${this.baseUrl}/user-active-sessions-and-devices`);
  }

  getUserSessions(targetUserId: string, justActiveSessions: boolean, currentPage: number = 1, pageSize: number = 10): Observable<Result<Paged<SessionSummaryDto>>> {
    return this.http.get<Result<Paged<SessionSummaryDto>>>(`${this.baseUrl}/user-sessions/${targetUserId}`, {
      params: {
        justActiveSessions: justActiveSessions.toString(),
        currentPage: currentPage.toString(),
        pageSize: pageSize.toString()
      }
    });
  }

  getSessionDetails(sessionId: string): Observable<Result<SessionDetailsDto>> {
    return this.http.get<Result<SessionDetailsDto>>(`${this.baseUrl}/sessions-details/${sessionId}`);
  }

  logoutFromSession(command: LogOutFromSessionCommand): Observable<Result<void>> {
    return this.http.post<Result<void>>(`${this.baseUrl}/logout-from-session`, command);
  }

  logoutFromOtherDevices(): Observable<Result<void>> {
    return this.http.post<Result<void>>(`${this.baseUrl}/logout-from-other-devices`, {});
  }

  logout(): Observable<Result<void>> {
    return this.http
      .post<Result<void>>(`${this.baseUrl}/logout`, {})
      .pipe(finalize(() => {
        this.authenticationService.removeToken();
      }));
  }

  refreshTokens(): Observable<Result<AuthTokenDto>> {
    return this.http.get<Result<AuthTokenDto>>(`${this.baseUrl}/refresh-tokens`);
  }
}