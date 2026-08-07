import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment.development';
import { map, Observable } from 'rxjs';
import { UserAccount } from '../contracts/user-account.dto';
import { Result } from '../../../../shared/contracts/result';
import { PersonalInformationDto } from '../contracts/user-personal-informations.dto';
import { ChangeUsernameRequest } from '../contracts/change-username-request.dto';
import { ChangePreferredLanguageRequest } from '../contracts/change-preferred-language-request.dto';
import { SupportedLanguage } from '../../../../shared/contracts/supported-language.enum';

@Injectable({
  providedIn: 'root',
})
export class UserManagementService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl + 'identity/users/userManagement'

  public getUserAccounts(): Observable<Result<UserAccount>> {
    return this.http.get<Result<UserAccount>>(`${this.apiUrl}/user-account`);
  }

  public getPersonalInformation(): Observable<PersonalInformationDto | null> {
    console.trace("getPersonalInformation");
    
    return this.http.get<Result<PersonalInformationDto>>(`${this.apiUrl}/user-information`)
      .pipe(map(
        result => result.value ?? null
      ));
  }

  public checkUsernameAvailabilty(username: string): Observable<Result<boolean>> {
    return this.http.get<Result<boolean>>(`${this.apiUrl}/username-available?username=${username}`);
  }

  public changeUserName(username: string): Observable<Result<string>> {
    const request: ChangeUsernameRequest = {
      newUsername: username
    }
    return this.http.put<Result<string>>(`${this.apiUrl}/update-username`, request);
  }

  public changePreferredLanguage(language: SupportedLanguage): Observable<Result<null>> {
    const request: ChangePreferredLanguageRequest = {
      preferredLanguage: language
    };
    return this.http.post<Result<null>>(`${this.apiUrl}/update-user-preferred-language`, request);
  }
}
