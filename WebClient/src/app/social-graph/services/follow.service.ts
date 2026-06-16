import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { ProfileBaseDto } from '../dtos/profile-base-dto.dto';

@Injectable({
  providedIn: 'root',
})
export class FollowService {
  private readonly api = inject(HttpClient);
  private apiUrl = environment.apiUrl + 'social-graph/follow';

  constructor() { }

  public getSuggestedFollowings(): Observable<ProfileBaseDto[]> {
    return this.api.get<ProfileBaseDto[]>(`${this.apiUrl}/suggested-followings`);
  }

}
