import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { Result } from '../../../../shared/contracts/result';
import { ChangePasswordRequestDto } from '../contracts/change-password-request.dto';
import { AuthTokensDto } from '../../../shared/contracts/auth-tokens.dto';

@Injectable({
  providedIn: 'root',
})
export class PasswordManagementService {
  private apiUrl = environment.apiUrl + 'identity/password-management/PasswordManagement';
  private http = inject(HttpClient);

  public changePassword(request: ChangePasswordRequestDto): Observable<Result<AuthTokensDto>> {
    return this.http.post<Result<AuthTokensDto>>(`${this.apiUrl}/change-password`, request);
  }
}
