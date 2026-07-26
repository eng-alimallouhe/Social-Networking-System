import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { Result } from '../../../../shared/contracts/result';
import { ForceRevokeUserSessionsRequest } from '../contracts/force-revoke-user-sessions-request.dto';
import { LogOutFromSessionRequest } from '../contracts/log-out-from-session-request.dto';
import { SessionDetailsDto } from '../contracts/session-details.dto';
import { UserActiveSessionsAndDevicesResult } from '../contracts/user-active-sessions-and-devices-result.dto';
import { SessionSummaryDto } from '../contracts/session-summary.dto';
import { Paged } from '../../../../shared/contracts/paged';


@Injectable({
  providedIn: 'root'
})
export class SessionsManagementService {

  private readonly baseUrl =
    `${environment.apiUrl}identity/security-sessions/SessionsManagement`;

  constructor(private readonly http: HttpClient) { }

  forceRevokeUserSessions(request: ForceRevokeUserSessionsRequest): Observable<Result> {
    return this.http.post<Result>(
      `${this.baseUrl}/force-revoke-user-sessions`,
      request
    );
  }

  logout(): Observable<Result> {
    return this.http.post<Result>(
      `${this.baseUrl}/logout`,
      {}
    );
  }

  logoutFromSession(request: LogOutFromSessionRequest): Observable<Result> {
    return this.http.post<Result>(
      `${this.baseUrl}/logout-from-session`,
      request
    );
  }

  logoutFromOtherDevices(): Observable<Result> {
    return this.http.post<Result>(
      `${this.baseUrl}/logout-from-other-devices`,
      {}
    );
  }

  getSessionDetails(sessionId: string): Observable<Result<SessionDetailsDto>> {
    return this.http.get<Result<SessionDetailsDto>>(
      `${this.baseUrl}/sessions-details/${sessionId}`
    );
  }

  getUserActiveSessionsAndDevices(): Observable<Result<UserActiveSessionsAndDevicesResult>> {
    return this.http.get<Result<UserActiveSessionsAndDevicesResult>>(
      `${this.baseUrl}/user-active-sessions-and-devices`
    );
  }

  getUserSessions(targetUserId: string): Observable<Result<Paged<SessionSummaryDto>>> {
    return this.http.get<Result<Paged<SessionSummaryDto>>>(
      `${this.baseUrl}/user-sessions/${targetUserId}`
    );
  }
}